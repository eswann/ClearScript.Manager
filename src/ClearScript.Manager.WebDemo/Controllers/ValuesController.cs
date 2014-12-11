using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ClearScript.Manager.WebDemo.Models;

namespace ClearScript.Manager.WebDemo.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public async Task<dynamic> Post([FromBody]Script script)
        {
            var scriptId = Guid.NewGuid();
            using (var scope = new ManagerScope())
            {
                //dynamic host = new ExpandoObject();
                var host = new TestModel();
                var option = new ExecutionOptions
                    {
                        HostObjects = new List<HostObject> {new HostObject {Name = "host", Target = host}}
                    };
                await scope.RuntimeManager.ExecuteAsync(scriptId.ToString(), script.Text, option);

                return host;
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
