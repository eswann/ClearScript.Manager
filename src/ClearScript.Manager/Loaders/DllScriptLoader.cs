using Microsoft.ClearScript;
using System;
using System.Reflection;

namespace JavaScript.Manager.Loaders
{
    /// <summary>
    /// Loads dll
    /// </summary>
    public class DllScriptLoader : IScriptLoader
    {
        public string Name { get { return nameof(DllScriptLoader); } }

        public bool ShouldUse(IncludeScript script)
        {
            try
            {
                if (!string.IsNullOrEmpty(script.Code) || string.IsNullOrEmpty(script.Uri))
                    return false;

                if (!script.Uri.ToLower().EndsWith(".dll"))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void LoadCode(IncludeScript script)
        {
            var asm = Assembly.LoadFrom(script.Uri);
            script.Exports = new HostTypeCollection(asm);
        }
    }
}