using System;
using System.Net.Http;
using System.Threading.Tasks;
using ClearScript.Manager.Loaders;

namespace ClearScript.Manager.Http.Loaders
{
    /// <summary>
    /// Loads scripts if they are file-system based scripts.
    /// </summary>
    public class HttpScriptLoader : IScriptLoader
    {
        public static void Register()
        {
            ScriptLoadManager.RegisterLoader(new HttpScriptLoader());
        }

        public bool ShouldUse(IncludeScript script)
        {
            if (!string.IsNullOrEmpty(script.Code))
                return false;

            if (Uri.IsWellFormedUriString(script.Uri, UriKind.RelativeOrAbsolute))
            {
                var uri = new Uri(script.Uri);
                if (!uri.IsFile)
                    return true;
            }

            return false;
        }

        public async Task LoadCodeAsync(IncludeScript script)
        {
            using (var httpClient = new HttpClient())
            {
                script.Code = await httpClient.GetStringAsync(script.Uri);
            }
        }
    }
}