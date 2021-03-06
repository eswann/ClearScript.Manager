﻿using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace ClearScript.Manager.Extensions
{
    /// <summary>
    /// Extension methods to get properties of dynamic objects.
    /// </summary>
    internal static class DynamicExtensions
    {
        private static readonly ConcurrentDictionary<string, Delegate> _delegateCache = new ConcurrentDictionary<string, Delegate>();

        /// <summary>
        /// Gets a property of the dynamic object.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="member">Member to retrieve.</param>
        /// <returns>The requested property value.</returns>
        /// <exception cref="ArgumentNullException">If source or member are null.</exception>
        public static object GetProperty(this object source, string member)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (member == null) throw new ArgumentNullException("member");

            Type scope = source.GetType();
            var provider = source as IDynamicMetaObjectProvider;
            if (provider != null)
            {
                var objectType = typeof (object);
                var cacheKey = objectType.FullName + member;
                Delegate del;

                if (!_delegateCache.TryGetValue(cacheKey, out del))
                {
                    ParameterExpression param = Expression.Parameter(typeof (object));
                    DynamicMetaObject mobj = provider.GetMetaObject(param);
                    var binder = (GetMemberBinder) Binder.GetMember(0, member, scope, new[] {CSharpArgumentInfo.Create(0, null)});
                    DynamicMetaObject ret = mobj.BindGetMember(binder);
                    BlockExpression final = Expression.Block(Expression.Label(CallSiteBinder.UpdateLabel), ret.Expression);
                    LambdaExpression lambda = Expression.Lambda(final, param);
                    del = lambda.Compile();
                    _delegateCache.TryAdd(cacheKey, del);
                }

                return del.DynamicInvoke(source);
            }
            return source.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance).GetValue(source, null);
        }

    }
}