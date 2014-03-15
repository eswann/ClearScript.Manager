using ClearScript.Manager;
using Microsoft.ClearScript;
using NUnit.Framework;

namespace Stratoflow.ClearScript.Test
{
    [TestFixture]
    public class WhenExecutingBasicJavaScript
    {
        [SetUp]
        public void TestSetUp()
        {
            ManagerPool.InitializeCurrentPool(new ManualManagerSettings());
        }

        [Test]
        public async void Script_With_No_Subject_Can_Be_Executed()
        {
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("test", "var i = 0; i++;");
        }

        [Test]
        public async void Script_Can_Write_To_Console()
        {
            var manager = new RuntimeManager(new ManualManagerSettings()) {AddConsoleReference = true};

            await manager.ExecuteAsync("test", "Console.WriteLine('wrote this to the console');");
        }

        [Test]
        public async void Javascript_Error_Is_Raised()
        {
            var manager = new RuntimeManager(new ManualManagerSettings());

            Assert.Throws<ScriptEngineException>(async () =>
            await manager.ExecuteAsync("test", "Console.WriteLine('wrote this to the console');"));
        }

        //[Test]
        //public async void Script_With_Dynamic_Subject_Can_Be_Executed()
        //{
        //    dynamic subject = new ExpandoObject();
        //    subject.Name = "Name";
        //    subject.Count = 3;

        //    var activity = new Activity
        //    {
        //        AddConsoleReference = true,
        //        Script = "subject.data.Count = 10;"
        //    };

        //    await activity.ExecuteAsync(subject);

        //    activity.Status.ShouldEqual(ActivityStatus.Success);

        //    Assert.AreEqual(subject.Name, "Name");
        //    Assert.AreEqual(subject.Count, 10);
        //}



        //[Test]
        //public async void Script_With_Error_Returns_Last_Error()
        //{
        //    var subject = new TestObject();

        //    var activity = new Activity
        //    {
        //        AddConsoleReference = true,
        //        Script = "subject.data.MissingProperty = 10;"
        //    };

        //    await activity.ExecuteAsync(subject);

        //    activity.Status.ShouldEqual(ActivityStatus.Error);
        //    activity.LastError.ShouldNotBeNull();
        //}

        //[Test]
        //public async void Script_Exceeding_Pool_Count_Waits()
        //{
        //    var subject = new TestObject();

        //    //Set the manager max count
        //    RuntimePool.InitializeCurrentPool(new ManualRuntimeSettings { RuntimeMaxCount = 2 });

        //    const string script = "Console.WriteLine('Started {0}:' + new Date().toJSON()); " +
        //                          "var now = new Date().getTime(); while(new Date().getTime() < now + 1000){{ /* do nothing */ }}; " +
        //                          "Console.WriteLine('finished {0}:' + new Date().toJSON());";

        //    var activity1 = new Activity
        //    {
        //        AddConsoleReference = true,
        //        Script = string.Format(script, "first")
        //    };

        //    var activity2 = new Activity
        //    {
        //        AddConsoleReference = true,
        //        Script = string.Format(script, "second")
        //    };

        //    var activity3 = new Activity
        //    {
        //        AddConsoleReference = true,
        //        Script = string.Format(script, "third")
        //    };

        //    var startDate = DateTime.UtcNow;

        //    await Task.WhenAll(
        //        Task.Run(() => activity1.ExecuteAsync(subject)),
        //        Task.Run(() => activity2.ExecuteAsync(subject)),
        //        Task.Run(() => activity3.ExecuteAsync(subject)));

        //    DateTime.UtcNow.Subtract(startDate).ShouldBeGreaterThan(new TimeSpan(0,0,2));

        //    activity3.Status.ShouldEqual(ActivityStatus.Success);
        //}

        //[Test]
        //public async void Script_Exceeding_Timeout_Errs()
        //{
        //    var subject = new TestObject();

        //    //Set the manager max count
        //    RuntimePool.InitializeCurrentPool(new ManualRuntimeSettings { ScriptTimeoutMilliSeconds = 500 });

        //    var activity = new Activity
        //    {
        //        AddConsoleReference = true,
        //        Script = "Console.WriteLine('Started:' + new Date().toJSON()); " +
        //                "var now = new Date().getTime(); while(new Date().getTime() < now + 1000){{ /* do nothing */ }}; " +
        //                "Console.WriteLine('Finished:' + new Date().toJSON());"
        //    };

        //    await activity.ExecuteAsync(subject);

        //    activity.Status.ShouldEqual(ActivityStatus.Error);
        //    activity.LastError.Message.ShouldContain("timeout");
        //}

      

    }
}