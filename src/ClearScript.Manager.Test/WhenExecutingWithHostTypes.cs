using System.Collections.Generic;
using JavaScript.Manager;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenExecutingWithHostTypes
    {
        [Test]
        public async void Script_Referencing_Specific_Type_Executes()
        {

            var manager = new RuntimeManager(new ManualManagerSettings());

            var hostType = new HostType
            {
                Name = "MathStuff",
                Type = typeof(System.Math)
            };

            var subject = new TestObject();

            await manager.ExecuteAsync("test", "subject.Result = MathStuff.Pow(10,2);", 
                new List<HostObject> { new HostObject { Name = "subject", Target = subject } }, 
                new List<HostType> { hostType });

            subject.Result.ShouldEqual(100);

        }

        public class TestObject
        {
            private string _name = "Name";

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public decimal Result { get; set; }
        } 

    }
}