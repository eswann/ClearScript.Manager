using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public static class DynamicExtensions
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


        public static bool TryGetMember(this DynamicObject source, string name, out object outField)
        {

            outField = ((dynamic)source).field;
            return !(outField is Microsoft.ClearScript.Undefined);
        }

        public static dynamic AsDynamic(this DynamicObject obj)
        {
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> GetDynamicProperties(this DynamicObject obj)
        {
            if (obj == null)
            {
                yield break;
            }
            foreach (var varName in obj.GetDynamicMemberNames())
            {
                object prop = obj.GetMember<object>(varName);
                yield return new KeyValuePair<string, object>(varName, prop);
            }
        }

        public static T GetMember<T>(this DynamicObject source, string name, T defaultValue = default(T))
        {
            Object outField;

            if (source.TryGetMember(new SimpleGetMemberBinder(name), out outField))
            {
                if (outField is Microsoft.ClearScript.Undefined)
                {
                    return defaultValue;
                }
                return (T)outField;
            }

            return defaultValue;
        }

        public static T GetMember<T>(this DynamicObject source, string name, Func<object, T> converter, T defaultValue = default(T))
        {
            Object outField;

            if (source.TryGetMember(new SimpleGetMemberBinder(name), out outField))
            {
                if (outField is Microsoft.ClearScript.Undefined)
                {
                    return defaultValue;
                }
                return converter(outField);
            }

            return defaultValue;
        }
    }

    internal class SimpleGetMemberBinder : GetMemberBinder
    {
        public SimpleGetMemberBinder(string name)
            : base(name, true)
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            return null;
        }
    }
}