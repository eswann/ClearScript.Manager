using System.Collections.Generic;
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
                    Scripts = new List<IncludeScript> {new IncludeScript {Code = preScript, Name = "testScript"}}
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
                    Scripts = new List<IncludeScript> {new IncludeScript {Uri = ".\\TestIncludeScript.js", Name = "testScript"}}
                });

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
            subject.TestString.ShouldEqual("test string1");
        }

        [Test]
        public async void Script_With_Http_Include_Is_Included()
        {
            var subject = new TestObject();
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("testscript", "subject.Count = 10; subject.TestString = x;",
                new ExecutionOptions
                {
                    HostObjects = new List<HostObject> {new HostObject {Name = "subject", Target = subject}},
                    Scripts = new List<IncludeScript> {new IncludeScript {Uri = "http://localhost:9510/TestIncludeScript.js", Name = "testScript"}}
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