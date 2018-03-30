﻿using JavaScript.Manager;
using JavaScript.Manager.Extensions;
using JavaScript.Manager.Http.Packages;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Log.Packages;
using JavaScript.Manager.Sql.AntOrm;
using JavaScript.Manager.Sql.Packages;
using JavaScript.Manager.Tabris;
using Microsoft.ClearScript;
using NUnit.Framework;
using Should;
using System.Net;
using System.Threading.Tasks;

namespace ClearScript.Manager.Http.Test
{
    [TestFixture]
    public class WhenMakingHttpCall
    {
        [SetUp]
        public void Setup()
        {
        }

       

        [Test]
        public async void Basic_Http_Get_Body_Is_Retrieved()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            HttpPackageHelpers.RegisterPackage(manager.RequireManager);

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

            HttpPackageHelpers.RegisterPackage(manager.RequireManager);

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });

            var scriptAwaiter = new ScriptAwaiter();
            options.HostObjects.Add(new HostObject { Name = "scriptAwaiter", Target = scriptAwaiter });
            options.HostObjects.Add(new HostObject { Name = "cookieA", Target = new CookieContainer() });

            var code = "var requestFactory = require('javascript_request_factory');" +
                         "var http = requestFactory.create({url:'http://www.baidu.com/'});" +
                        "var data = encodeURIComponent('errorMsg=&to=http%253A%252F%252Fwww.zhonghuasuan.com%252F&token=5b9c1a3c6f2db8c737b7788ac560a397&account=111111&password=111111');" +
                       "Console.WriteLine('aaaa111'); var aa = http.getString({timeout:10,headers:{token:'aaaaaatoken'},cookieContainer:cookieA});Console.WriteLine(aa);";

            await manager.ExecuteAsync("testScript", code, options);

        }

        [Test]
        public async void TestSql1()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            SqlPackageHelpers.RegisterPackage(manager.RequireManager,new AntOrmDbExecutor());

            manager.AddConsoleReference = true;
            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });
            var code = "var dbFactory = require('javascript_sql_factory');" +
                       "var content = this.dbFactory.create({name:'testorm',type:'mysql'});" +
                       //"subject.StatusCode = content.ExecuteNonQuery(\"update school set address ='1' where id = 1\");";
                       "var arr = content.insert(\"insert into school (name,address,datachange_lasttime) values (@name,@address,now())\",{name:'test111',address:'wowowo'});";

            await manager.ExecuteAsync("testScript", code, options);

            subject.StatusCode.ShouldEqual(0);
        }

        [Test]
        public async void TestLog1()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });
            manager.AddConsoleReference = true;
            LogPackageHelpers.RegisterPackage(manager.RequireManager);

            var options = new ExecutionOptions();
            options.HostObjects.Add(new HostObject { Name = "subject", Target = subject });
            var code = "var logFactory = require('javascript_log_factory');" +
                       "subject.Name = '111';" +
                       "var log = this.logFactory.create({trace:true});" +
                       //"subject.StatusCode = content.ExecuteNonQuery(\"update school set address ='1' where id = 1\");";
                       "try{ aa.ttt =1}catch(err){log.info(err)}";

            await manager.ExecuteAsync("testScript", code, options);

        }

        [Test]
        public async void Testtabris()
        {
            RequireManager requireManager = new RequireManager();
            Tabris.Register(requireManager);
            ManagerPool.InitializeCurrentPool(new ManagerSettings());
            using (var scope = new ManagerScope(requireManager))
            {

                var code = "var aaaaa = require('./TestMainScript.js');";

                //           "var log = this.tabris.create('LOG');" +
                //           "try{ aa.ttt =1}catch(err){log.info(err)}";
                //code = "var tabris;" + "(function (){\n  tabris = tabris || require('javascript_tabris'); \n" + code + "\n})();";
                await scope.RuntimeManager.ExecuteAsync("btnExcutor_Click", code);

                code = "var log = this.tabris.create('LOG');" +
                           "try{ aa.ttt =1}catch(err){log.info(err)}";
                code = "var tabris;" + "(function (){\n  tabris = tabris || require('javascript_tabris'); \n" + code + "\n})();";
                await scope.RuntimeManager.ExecuteAsync("btnExcutor_Click", code);
            }
            

            //RequireManager.ClearPackages();

        }

        [Test]
        public async void Testtabris1()
        {
            RequireManager requireManager = new RequireManager();
            ManagerPool.InitializeCurrentPool(new ManagerSettings());
            JavaScript.Manager.Tabris.Tabris.Register(requireManager, new JavaScript.Manager.Tabris.TabrisOptions
            {
            });
            using (var scope = new ManagerScope(requireManager))
            {

                var code = @"var tabris;
(function (){
  tabris = tabris || require('javascript_tabris'); 
try{
var lib = require('../TestDll.dll');
var myClrObject = new lib.TestDll.MyClass('tttt');
myClrObject.SayHello(); 
}catch(err){
host.err=err.message;
host.ex=err;
}
})();";
                await scope.RuntimeManager.ExecuteAsync("btnExcutor_Click", code);

            }


            //RequireManager.ClearPackages();

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