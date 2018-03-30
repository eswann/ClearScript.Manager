using System;
using System.IO;

namespace JavaScript.Manager.Loaders
{
    /// <summary>
    /// Loads scripts if they are file-system based scripts.
    /// </summary>
    public class FileScriptLoader : IScriptLoader
    {
        public string Name { get { return nameof(FileScriptLoader); } }

        public bool ShouldUse(IncludeScript script)
        {
            try
            {
                if (!string.IsNullOrEmpty(script.Code) || string.IsNullOrEmpty(script.Uri))
                    return false;

                if (script.RequiredPackage != null &&
                    !script.RequiredPackage.RequiredPackageType.Equals(RequiredPackageType.Default))
                {
                    return false;
                }

                if (!File.Exists(script.Uri))
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
            using (var reader = File.OpenText(script.Uri))
            {
                script.Code = reader.ReadToEnd();
            }
        }
    }
}