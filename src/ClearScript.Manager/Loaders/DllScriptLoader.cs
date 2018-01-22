using System;
using System.IO;

namespace JavaScript.Manager.Loaders
{
    /// <summary>
    /// Loads dll
    /// </summary>
    public class DllScriptLoader : IScriptLoader
    {
        public string Name { get { return "EmbeddedScriptLoader"; } }

        public bool ShouldUse(IncludeScript script)
        {
            try
            {
                if (!string.IsNullOrEmpty(script.Code) || script.RequiredPackage == null)
                    return false;

                if (!script.RequiredPackage.ScriptUri.ToLower().EndsWith(".dll"))
                {
                    return false;
                }

                if (script.Uri.Contains("/") || script.Uri.Contains("\\"))
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
            script.Code = script.RequiredPackage.GetEmbeddedAsset(script.RequiredPackage.ScriptUri);
            if (string.IsNullOrEmpty(script.Code))
            {
                throw new FileNotFoundException("Embedded Resource not found or content is empty : " + script.RequiredPackage.ScriptUri);
            }
        }
    }
}