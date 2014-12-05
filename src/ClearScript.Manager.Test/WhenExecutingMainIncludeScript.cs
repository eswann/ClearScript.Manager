using System.Collections.Generic;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenExecutingMainIncludeScript
    {
        [Test]
        public async void Script_With_String_Code_Is_Run()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync(new List<IncludeScript> { new IncludeScript { Code = "subject.Count = 10; subject.TestString = 'test string1';", ScriptId = "testScript" } },
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } },
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
        }

        [Test]
        public async void Multiple_Scripts_With_String_Code_Are_Run()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync(new List<IncludeScript>
            {
                new IncludeScript { Code = "subject.Count = 10;", ScriptId = "testScript" },
                new IncludeScript { Code = "subject.TestString = 'test string1';", ScriptId = "testScript2" }
            },
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } },
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
        }

        [Test]
        public async void Multiple_Scripts_Are_Run_First_To_Last()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync(new List<IncludeScript>
            {
                new IncludeScript { Code = "subject.Count = 10;", ScriptId = "testScript" },
                new IncludeScript { Code = "subject.TestString = 'test string1';", ScriptId = "testScript2" },
                new IncludeScript { Code = "subject.TestString = 'test string3';", ScriptId = "testScript3" }
            },
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } },
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string3");
        }

        [Test]
        public async void Multiple_Scripts_Types_Are_Run_First_To_Last()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync(new List<IncludeScript>
                {
                    new IncludeScript {Uri = ".\\TestMainScript.js", ScriptId = "testScript"},
                    new IncludeScript {Code = "subject.TestString = 'test string3';", ScriptId = "testScript3"},
                    new IncludeScript {Uri = "https://gist.githubusercontent.com/eswann/62fc90e542f7697263db/raw/026c42ef0043a70a8c3b720e3026ecee9c691a06/gistfile1.txt", ScriptId = "TestScript4"}
                },
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(20);
            subject.TestString.ShouldEqual("test string3");
        }

        [Test]
        public async void Script_With_File_Reference_Is_Run()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync(new List<IncludeScript> { new IncludeScript { Uri = ".\\TestMainScript.js", ScriptId = "testScript" } },
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } },
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
        }

        [Test]
        public async void Script_With_Http_Reference_Is_Run()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync(new List<IncludeScript> { new IncludeScript { Uri = "https://gist.githubusercontent.com/eswann/66796164c1904b6135ea/raw/5491642164a45ae34af2926137950de3249d1e91/gistfile1.txt", ScriptId = "testScript" } },
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } },
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
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