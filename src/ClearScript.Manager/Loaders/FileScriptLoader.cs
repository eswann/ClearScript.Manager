using System;
using System.IO;
using System.Threading.Tasks;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Loads scripts if they are file-system based scripts.
    /// </summary>
    public class FileScriptLoader : IScriptLoader
    {
        public bool ShouldUse(IncludeScript script)
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

        public async Task LoadCodeAsync(IncludeScript script)
        {
            using (var reader = File.OpenText(script.Uri))
            {
                script.Code = await reader.ReadToEndAsync();
            }
        }
    }
}