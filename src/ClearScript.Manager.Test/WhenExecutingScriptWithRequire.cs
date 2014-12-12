using System.Collections.Generic;
using ClearScript.Manager.Loaders;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenExecutingScriptWithRequire
    {
        [SetUp]
        public void Setup()
        {
            RequireManager.ClearPackages();
        }

        [Test]
        public async void Script_With_Require_Is_Used()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings {ScriptTimeoutMilliSeconds = 0});

            RequireManager.RegisterPackage(new RequiredPackage { PackageId = "testRequire", ScriptUri = ".\\TestRequire.js" });

            await manager.ExecuteAsync("testscript",
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
        public async void Script_With_Nested_Require_Is_Used()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings {ScriptTimeoutMilliSeconds = 0});

            RequireManager.RegisterPackage(new RequiredPackage { PackageId = "testRequire", ScriptUri = ".\\TestRequire.js" });
            RequireManager.RegisterPackage(new RequiredPackage
            {
                PackageId = "testParentRequire",
                ScriptUri = ".\\TestParentRequire.js"
            });

            await
                manager.ExecuteAsync("testscript",
                    "var testObject = require('testParentRequire'); subject.Count = testObject.getNumber(); subject.TestString = testObject.getText();",
                    new ExecutionOptions
                    {
                        HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                    });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(500);
            subject.TestString.ShouldEqual("testText");
        }

        [Test]
        public async void Script_With_Multiple_Requires_Is_Used()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings {ScriptTimeoutMilliSeconds = 0});

            RequireManager.RegisterPackage(new RequiredPackage { PackageId = "testRequire", ScriptUri = ".\\TestRequire.js" });
            RequireManager.RegisterPackage(new RequiredPackage { PackageId = "testRequire2", ScriptUri = ".\\TestRequire2.js" });

            await
                manager.ExecuteAsync("testscript",
                    "var testObject = require('testRequire'); var testObject2 = require('testRequire2');subject.Count = testObject2.getNumber(); subject.TestString = testObject.getText();",
                    new ExecutionOptions
                    {
                        HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                    });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(300);
            subject.TestString.ShouldEqual("testText");
        }

        [Test]
        public async void Require_Specified_As_Script_Local_Path_Is_Used()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings {ScriptTimeoutMilliSeconds = 0});

            await
                manager.ExecuteAsync("testscript",
                    @"var testObject = require('.\\TestRequire.js'); subject.Count = 10; subject.TestString = testObject.getText();",
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
                    @"var testObject = requireNamed('testRequire', '.\\TestDiffNameRequire.js'); subject.Count = 10; subject.TestString = testObject.getText();",
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