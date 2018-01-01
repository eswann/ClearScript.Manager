using JavaScript.Manager;
using JavaScript.Manager.Loaders;
using NUnit.Framework;
using Should;
using System.Collections.Generic;

namespace ClearScript.Manager.Http.Test
{
    [TestFixture]
    public class WhenExecutingScriptWithRequire
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async void Script_With_Require_Is_Used()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings {ScriptTimeoutMilliSeconds = 0});

            manager.RequireManager.RegisterPackage(new RequiredPackage { PackageId = "testRequire", ScriptUri = "https://gist.githubusercontent.com/eswann/76ecaba02dee33cf26b4/raw/bcafe0a389c84ba44d6ee1661e66b2213aa2ffa0/testRequire" });

            await
                manager.ExecuteAsync("testscript",
                    "var testObject = require('testRequire'); subject.Count = 10; subject.TestString = testObject.getText();",
                    new ExecutionOptions
                    {
                        HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                    });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("testText");
        }

        [Test]
        public async void Require_Specified_As_Script_HttpPath_Path_Is_Used()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings {ScriptTimeoutMilliSeconds = 0});

            await
                manager.ExecuteAsync("testscript",
                    @"var testObject = require('https://gist.githubusercontent.com/eswann/76ecaba02dee33cf26b4/raw/bcafe0a389c84ba44d6ee1661e66b2213aa2ffa0/testRequire'); subject.Count = 10; subject.TestString = testObject.getText();",
                    new ExecutionOptions
                    {
                        HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                    });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("testText");
        }

        [Test]
        public async void Require_Named_With_Explicit_Name_Succeeds()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });

            await
                manager.ExecuteAsync("testscript",
                    @"var testObject = requireNamed('testRequire', 'https://gist.githubusercontent.com/eswann/3ff5ec2bcc63b7d2cdaa/raw/26e30f6a9761dbbe5634b2de1549c4a455b7a7d6/TestDiffNameRequire.js'); subject.Count = 10; subject.TestString = testObject.getText();",
                    new ExecutionOptions
                    {
                        HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } },
                    });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("testText");
        }

    }
    

    public class TestObject
    {
        private string _name = "Name";

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string TestString { get; set; }

        public int Count { get; set; }
    } 
}