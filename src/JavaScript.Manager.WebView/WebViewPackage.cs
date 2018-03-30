using JavaScript.Manager.Loaders;
using System;

namespace JavaScript.Manager.WebView
{
    public class WebViewExcutorPackage : RequiredPackage
    {
        public WebViewExcutorPackage(object webViewExecutor)
        {
            PackageId = "javascript_webview_factory_webviewExecutor";
            var type = webViewExecutor as Type;
            if (type != null)
            {
                HostObjects.Add(new HostObject
                {
                    Name = "javascript_webview_factory_webviewExecutor",
                    Target = Activator.CreateInstance(type)
                });
            }
            else
            {
                HostObjects.Add(new HostObject
                {
                    Name = "javascript_webview_factory_webviewExecutor",
                    Target = webViewExecutor
                });
            }
        }
    }

    public class WebViewPackage : RequiredPackage
    {
        public WebViewPackage()
        {
            PackageId = "javascript_webview_factory";
            ScriptUri = "JavaScript.Manager.WebView.Scripts.webview.js";
            RequiredPackageType = RequiredPackageType.EmbeddedFile;
        }
    }
}
