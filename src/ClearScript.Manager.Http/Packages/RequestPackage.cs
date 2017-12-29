using System.IO;
using JavaScript.Manager.Loaders;

namespace JavaScript.Manager.Http.Packages
{
    public class RequestPackage : RequiredPackage
    {
        public RequestPackage()
        {
            PackageId = "request";
            ScriptUri = "ClearScript.Manager.Http.Scripts.request.js";
        }

       
    }
}