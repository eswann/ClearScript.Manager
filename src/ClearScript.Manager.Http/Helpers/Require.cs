using System;
using System.IO;
using ClearScript.Manager.Http.Helpers.Node;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager.Http.Helpers
{
    public class Require
    {
        private readonly V8ScriptEngine _engine;
        private readonly string _path;

        public Require(V8ScriptEngine engine, string path)
        {
            engine.AddHostType("Buffer", typeof(NodeBuffer));
            _engine = engine;
            _path = path;
            Init();
        }

        private void Init()
        {
            _engine.AddHostObject("require", new Func<string, object>(RequireScript));
        }

        public object RequireScript(string script)
        {
            switch (script)
            {
                case "http":
                    {
                        return new NodeHttp();
                    }
                case "Buffer":
                    {
                        break;
                    }
            }
            var scriptFile = new FileInfo(_path + script);


            _engine.Evaluate(scriptFile.FullName, false, "module={};\r\n" + scriptFile.OpenText().ReadToEnd());
            object module = _engine.Script.module.exports;

            return module;
        }
    }
}
