using System;
using System.Linq;
using System.Threading.Tasks;
using JavaScript.Manager;
using JavaScript.Manager.Extensions;
using JavaScript.Manager.Http.Packages;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Log.Packages;
using JavaScript.Manager.Sql.Packages;
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
            RequireManager.ClearPackages();
        }

        [Test]
        public async void Basic_Http_Get_Succeeds()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings{ScriptTimeoutMilliSeconds = 0});

            HttpPackageHelpers.RegisterPackage();

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

        [Test]
        public async void Basic_Http_Get_Body_Is_Retrieved()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            HttpPackageHelpers.RegisterPackage();

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
        public async Task Basic_Http_Get_Headers_Are_Retrieved()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            HttpPackageHelpers.RegisterPackage();

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });

            var scriptAwaiter = new ScriptAwaiter();
            options.HostObjects.Add(new HostObject { Name = "scriptAwaiter", Target = scriptAwaiter });

            var code = "var requestFactory = require('javascript_request_factory');" +
                         "var http = requestFactory.create({url:'http://www.baidu.com/'});" +
                        "var data = encodeURIComponent('errorMsg=&to=http%253A%252F%252Fwww.zhonghuasuan.com%252F&token=5b9c1a3c6f2db8c737b7788ac560a397&account=111111&password=111111');" +
                       "Console.WriteLine('aaaa111'); var aa = http.getString({timeout:10,headers:{token:'aaaaaatoken'}});Console.WriteLine(aa);";

            await manager.ExecuteAsync("testScript", code, options);

        }

        [Test]
        public async void TestSql1()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            SqlPackageHelpers.RegisterPackage();

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });
            var code = "var dbFactory = require('javascript_sql_factory');" +
                       "var content = this.dbFactory.create('testorm','mysql');" +
                       //"subject.StatusCode = content.ExecuteNonQuery(\"update school set address ='1' where id = 1\");";
                       "var arr = content.query(\"SELECT  DataChange_LastTime FROM school LIMIT 1\");Console.WriteLine(arr.ToString('yyyy-MM-dd'))";

            await manager.ExecuteAsync("testScript", code, options);

            subject.StatusCode.ShouldEqual(0);
        }

        [Test]
        public async void TestLog1()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            LogPackageHelpers.RegisterPackage();

            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });
            var code = "var logFactory = require('javascript_log_factory');" +
                       "var log = this.logFactory.create();" +
                       //"subject.StatusCode = content.ExecuteNonQuery(\"update school set address ='1' where id = 1\");";
                       "try{ aa.ttt =1}catch(err){log.info(err)}";

            await manager.ExecuteAsync("testScript", code, options);

            subject.StatusCode.ShouldEqual(0);
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