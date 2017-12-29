using System;
using System.IO;

namespace JavaScript.Manager.Loaders
{
    /// <summary>
    /// Loads scripts if they are file-system based scripts.
    /// </summary>
    public class FileScriptLoader : IScriptLoader
    {
        public string Name { get { return "FileScriptLoader"; } }

        public bool ShouldUse(IncludeScript script)
        {
            try
            {
                if (!string.IsNullOrEmpty(script.Code))
                    return false;

                if (Uri.IsWellFormedUriString(script.Uri, UriKind.RelativeOrAbsolute))
                {
                    var uri = new Uri(script.Uri);

                    if (!uri.IsFile)
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