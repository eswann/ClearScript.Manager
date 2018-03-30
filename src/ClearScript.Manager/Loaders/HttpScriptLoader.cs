using System;
using System.Net.Http;

namespace JavaScript.Manager.Loaders
{
    /// <summary>
    /// Loads scripts if they are file-system based scripts.
    /// </summary>
    public class HttpScriptLoader : IScriptLoader
    {
        public string Name { get { return nameof(HttpScriptLoader); } }

     
        public bool ShouldUse(IncludeScript script)
        {
            if (!string.IsNullOrEmpty(script.Code))
                return false;

            if (!script.Uri.ToLower().StartsWith("http://") && !script.Uri.ToLower().StartsWith("https://"))
            {
                return false;
            }

            if (Uri.IsWellFormedUriString(script.Uri, UriKind.RelativeOrAbsolute))
            {
                var uri = new Uri(script.Uri);
                if (!uri.IsFile)
                    return true;
            }
            return false;
        }

        public void LoadCode(IncludeScript script)
        {
            using (var httpClient = new HttpClient())
            {
                script.Code = httpClient.GetStringAsync(script.Uri).Result;
            }
        }
    }
}