using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ClearScript.Manager.Http.Helpers
{
    public static class DynamicObjectExtensions
    {
        public static bool HasField(this DynamicObject obj, string field)
        {
            return !(obj.AsDynamic().field is Microsoft.ClearScript.Undefined);
        }

        public static bool TryGetField(this DynamicObject obj, string field, out object outField)
        {

            outField = obj.AsDynamic().field;
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
                if (obj.TryGetField(varName, out prop))
                {
                    yield return new KeyValuePair<string, object>(varName, prop);
                }
            }
        }

        public static T GetField<T>(this DynamicObject obj, string field, T defaultValue = default(T))
        {
            Object outField;

            if (obj.TryGetMember(new SimpleGetMemberBinder(field), out outField))
            {
                if (outField is Microsoft.ClearScript.Undefined)
                {
                    return defaultValue;
                }
                return (T)outField;
            }

            return defaultValue;
        }

        public static T GetField<T>(this DynamicObject obj, string field, Func<object, T> converter, T defaultValue = default(T))
        {
            Object outField;

            if (obj.TryGetMember(new SimpleGetMemberBinder(field), out outField))
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
}