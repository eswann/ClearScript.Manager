using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ClearScript.Manager.Http.Helpers
{
    public static class DynamicExtensions
    {
        public static bool TryGetMember(this DynamicObject source, string name, out object outField)
        {
            
            outField = ((dynamic)source).field;
            return !(outField is Microsoft.ClearScript.Undefined);
        }

        public static dynamic AsDynamic(this DynamicObject obj)
        {
            return obj;
        }

        public static IEnumerable<KeyValuePair<string, object>> GetProperties(this DynamicObject obj)
        {
            if (obj == null)
            {
                yield break;
            }
            foreach (var varName in obj.GetDynamicMemberNames())
            {
                object prop;
                if (obj.TryGetMember(varName, out prop))
                {
                    yield return new KeyValuePair<string, object>(varName, prop);
                }
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