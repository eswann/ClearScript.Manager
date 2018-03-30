using Microsoft.ClearScript;

namespace JavaScript.Manager.Extensions
{
    internal static class EngineExtensions
    {
        public static ScriptEngine AddHostObject(this ScriptEngine engine, HostObject hostObject)
        {
            engine.AddHostObject(hostObject.Name, hostObject.Flags, hostObject.Target);

            return engine;
        }

        public static ScriptEngine AddHostType(this ScriptEngine engine, HostType hostType)
        {
            engine.AddHostType(hostType.Name, hostType.Flags, hostType.Type);

            return engine;
        }

        public static ScriptEngine ApplyOptions(this ScriptEngine engine, ExecutionOptions options)
        {
            if (options != null)
            {
                if (options.HostObjects != null)
                {
                    foreach (HostObject hostObject in options.HostObjects)
                    {
                        engine.AddHostObject(hostObject);
                    }
                }

                if (options.HostTypes != null)
                {
                    foreach (HostType hostType in options.HostTypes)
                    {
                        engine.AddHostType(hostType);
                    }
                }
            }

            return engine;
        }
    }
}