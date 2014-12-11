using System.Linq;
using ClearScript.Manager.Http.Helpers;
using ClearScript.Manager.Http.Packages;
using ClearScript.Manager.Loaders;
using Microsoft.ClearScript;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Http.Test
{
    [TestFixture]
    public class WhenMakingHttpCall
    {
        [SetUp]
        public void Setup()
        {
            Requirer.ClearPackages();
        }

        [Test]
        public async void Basic_Http_Get_Succeeds()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings{ScriptTimeoutMilliSeconds = 0});

            Requirer.RegisterPackage(new HttpPackage());
            Requirer.RegisterPackage(new RequestPackage());

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject {Name = "subject", Target = subject});

            var scriptAwaiter = new ScriptAwaiter();
            options.HostObjects.Add(new HostObject { Name = "scriptAwaiter", Target = scriptAwaiter });

            var code = "var request = require('request');" +
                       "request({url: 'http://api.icndb.com/jokes/random/1', json: true}," +
                       " function (error, response, body) {subject.StatusCode = response.statusCode; subject.Response = response; scriptAwaiter.Callback();});";

            await manager.ExecuteAsync("testScript", code, options);
            await scriptAwaiter.T;

            subject.StatusCode.ShouldEqual(200);
        }

        [Test, Ignore]
        public async void Basic_Http_Get_Body_Is_Retrieved()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            Requirer.RegisterPackage(new HttpPackage());
            Requirer.RegisterPackage(new RequestPackage());

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });

            var scriptAwaiter = new ScriptAwaiter();
            options.HostObjects.Add(new HostObject { Name = "scriptAwaiter", Target = scriptAwaiter });

            var code = "var request = require('request');" +
                       "request({url: 'http://api.icndb.com/jokes/random/1', json: true}," +
                       " function (error, response, body) {subject.Response = response; subject.Body = body; subject.Joke = body.value[0].joke; scriptAwaiter.Callback();});";

            await manager.ExecuteAsync("testScript", code, options);
            await scriptAwaiter.T;

            subject.Joke.ShouldNotBeNull();
        }

        [Test]
        public async void Basic_Http_Get_Headers_Are_Retrieved()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            Requirer.RegisterPackage(new HttpPackage());
            Requirer.RegisterPackage(new RequestPackage());

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });

            var scriptAwaiter = new ScriptAwaiter();
            options.HostObjects.Add(new HostObject { Name = "scriptAwaiter", Target = scriptAwaiter });

            var code = "var request = require('request');" +
                       "request({url: 'http://api.icndb.com/jokes/random/1', json: true}," +
                       " function (error, response, body) {subject.Response = response; subject.Headers = response.headers; scriptAwaiter.Callback();});";

            await manager.ExecuteAsync("testScript", code, options);
            await scriptAwaiter.T;

            subject.Headers.Count().ShouldBeGreaterThan(0);
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

            public string Joke { get; set; }

            public object Response { get; set; }

            public object Body { get; set; }

            public PropertyBag Headers { get; set; }

            public int Count { get; set; }
        } 

    }
}