using System.IO;
using ClearScript.Manager.Loaders;

namespace ClearScript.Manager.Http.Packages
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