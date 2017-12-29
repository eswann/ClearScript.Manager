using System;
using System.Linq;
using System.Threading.Tasks;
using ClearScript.Manager.Extensions;
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
            RequireManager.ClearPackages();
        }

        [Test]
        public async void Basic_Http_Get_Succeeds()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings{ScriptTimeoutMilliSeconds = 0});

            HttpPackageHelpers.RegisterRequestPackages();

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

            HttpPackageHelpers.RegisterRequestPackages();

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

            HttpPackageHelpers.RegisterRequestPackages();

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });

            var scriptAwaiter = new ScriptAwaiter();
            options.HostObjects.Add(new HostObject { Name = "scriptAwaiter", Target = scriptAwaiter });

            var code = "var request = require('request');" +
                        "var data = encodeURIComponent('errorMsg=&to=http%253A%252F%252Fwww.zhonghuasuan.com%252F&token=5b9c1a3c6f2db8c737b7788ac560a397&account=111111&password=111111');" +
                       "request({url: 'http://www.sf-express.com/sf-service-owf-web/service/rate?origin=A310105000&dest=A440306000&parentOrigin=A310105000&parentDest=A440306000&weight=1&time=2017-12-29T01%3A30%3A00%2B08%3A00&volume=0&queryType=2&lang=sc&region=cn&translate=',method:'GET', json: true,headers:{token:'aaaaaatoken'}}," +
                       " function (error, response, body) {  if(error){subject.Body=error;return;}; subject.Response = response;subject.Body=body; subject.Headers = response.headers; });";

            await manager.ExecuteAsync("testScript", code, options);

            subject.Headers.Count().ShouldBeGreaterThan(0);
        }

        [Test]
        public async void TestSql1()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            SqlPackageHelpers.RegisterSqlPackages();

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });
            var code = "var sql = require('sql');" +
                       "var content = new sql('testorm_sqlserver','sqlserver');"+
                       //"subject.StatusCode = content.ExecuteNonQuery(\"update school set address ='1' where id = 1\");";
                       "var arr = content.Query(\"select top 1 DataChange_LastTime from school\");Console.WriteLine(arr.ToString('yyyy-MM-dd'));subject.Server = ''+arr; content.UseTransaction(function(){content.Exec(\"update school set name='bbb' where id=4; \");content.Exec(\"insert into school (Name) VALUES ('test1')\")},{'level':'ReadCommitted',timeout:60})";

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