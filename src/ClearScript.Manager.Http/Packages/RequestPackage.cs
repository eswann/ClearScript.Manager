using System.IO;
using JavaScript.Manager.Loaders;

namespace JavaScript.Manager.Http.Packages
{
    public class RequestPackage : RequiredPackage
    {
        public RequestPackage()
        {
            PackageId = "javascript_request_factory";
            ScriptUri = "JavaScript.Manager.Http.Scripts.request.js";
        }

       
    }
}