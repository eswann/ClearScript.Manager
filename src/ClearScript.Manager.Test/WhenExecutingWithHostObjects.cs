using System.Collections.Generic;
using System.Dynamic;
using ClearScript.Manager;
using NUnit.Framework;
using Should;

namespace Stratoflow.ClearScript.Test
{
    [TestFixture]
    public class WhenExecutingWithHostObjects
    {

        [Test]
        public async void Script_With_Typed_Subject_Can_Be_Executed()
        {
            var subject = new TestObject();

            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("test", "subject.Count = 10;",
                new List<HostObject> {new HostObject {Name = "subject", Target = subject}}, null);

            subject.Name.ShouldEqual("Name");
            subject.Count.ShouldEqual(10);
        }

        [Test]
        public async void Script_With_Dynamic_Subject_Can_Be_Executed()
        {
            dynamic subject = new ExpandoObject();
            subject.Name = "Name";
            subject.Count = 0;

            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("test", "subject.Count = 10;",
                new List<HostObject> { new HostObject { Name = "subject", Target = subject } }, null);

            //Should craps out on dynamics
            Assert.AreEqual(subject.Name, "Name");
            Assert.AreEqual(subject.Count, 10);
        }


        public class TestObject
        {
            private string _name = "Name";

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public int Count { get; set; }
        } 
    }
}