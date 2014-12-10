using ClearScript.Manager.Http.Helpers;
using ClearScript.Manager.Http.Packages;
using ClearScript.Manager.Loaders;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Http.Test
{
    [TestFixture]
    public class WhenMakingHttpCall
    {
        [Test]
        public async void Basic_Http_Get_Succeeds()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings{ScriptTimeoutMilliSeconds = 0});

            Requirer.RegisterPackage(new HttpPackage());
            Requirer.RegisterPackage(new RequestPackage());

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions().AddHttpHelperObjects();
            options.HostObjects.Add(new HostObject {Name = "subject", Target = subject});

            var callbacker = new Callbacker();
            options.HostObjects.Add(new HostObject { Name = "callbacker", Target = callbacker });

            var code = "var request = require('request');" +
                       "request({url: 'http://api.icndb.com/jokes/random/1', json: true}," +
                       " function (error, response, body) {subject.StatusCode = response.statusCode; subject.Response = response; callbacker.Callback();});";

            await manager.ExecuteAsync("testScript", code, options);

            await callbacker.T;

            subject.StatusCode.ShouldEqual(200);

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

            public int StatusCode { get; set; }

            public string TestString { get; set; }

            public object Response { get; set; }

            public object Body { get; set; }

            public int Count { get; set; }
        } 

    }
}