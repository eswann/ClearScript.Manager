﻿using System.Collections.Generic;
using JavaScript.Manager;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenExecutingScriptWithIncludeScripts
    {
        [Test]
        public async void Script_With_String_Code_Include_Is_Included()
        {
            var preScript = "var x = 'test string1';";

            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("testscript", "subject.Count = 10; subject.TestString = x;",
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                    Scripts = new List<IncludeScript> {new IncludeScript {Code = preScript, ScriptId = "testScript2"}}
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
        }

        [Test]
        public async void Script_With_File_Include_Is_Included()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("testscript", "subject.Count = 10; subject.TestString = x;",
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                    Scripts = new List<IncludeScript> {new IncludeScript {Uri = ".\\TestIncludeScript.js", ScriptId = "testScript2"}}
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
        }

        [Test]
        public async void Included_Variables_Are_Accessible_Via_Script_Object()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            var engine = await manager.ExecuteAsync("testscript", "subject.Count = 10; subject.TestString = x;",
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } },
                    Scripts = new List<IncludeScript> { new IncludeScript { Uri = ".\\TestIncludeScript.js", ScriptId = "testScript2" } }
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
            engine.Script.x = "test string1";
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
}