using System.Collections.Generic;
using System.Threading.Tasks;
using ClearScript.Manager;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Http.Test
{
    [TestFixture]
    public class WhenMakingHttpCall
    {
        [Test, Ignore]
        public async void Basic_Http_Get_Succeeds()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions().AddHttpHelperObjects();
            options.HostObjects.Add(new HostObject {Name = "subject", Target = subject});

            var code = "subject.Count=30; " +
                       "request({url: 'http://api.icndb.com/jokes/random/1', json: true}," +
                       " function (error, response, body) {subject.StatusCode = response.StatusCode;});";

            await manager.ExecuteAsync("testScript", code, options);

            subject.StatusCode.ShouldNotBeNull();

        }

        public class TestObject
        {
            private string _name = "Name";

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Server { get; set; }

            public string StatusCode { get; set; }

            public string TestString { get; set; }

            public int Count { get; set; }
        } 

    }
}