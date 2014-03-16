using Microsoft.ClearScript;
using NUnit.Framework;

namespace ClearScript.Manager.Test
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

    }
}