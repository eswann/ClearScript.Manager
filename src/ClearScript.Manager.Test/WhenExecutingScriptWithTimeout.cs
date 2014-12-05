using Microsoft.ClearScript;
using NUnit.Framework;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenExecutingScriptWithTimeout
    {
        [Test]
        public void Script_Exceeding_Timeout_Errs()
        {
            var manager = new RuntimeManager(new ManualManagerSettings{ScriptTimeoutMilliSeconds = 500})
            {
                AddConsoleReference = true
            };

            var script = "Console.WriteLine('Started:' + new Date().toJSON()); " +
                         "var now = new Date().getTime(); while(new Date().getTime() < now + 1000){{ /* do nothing */ }}; " +
                         "Console.WriteLine('Finished:' + new Date().toJSON());";


            Assert.Throws<ScriptInterruptedException>(async () => await manager.ExecuteAsync("test", script));
    
        }


        [Test]
        public async void Script_Not_Exceeding_Timeout_Works()
        {
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 500 })
            {
                AddConsoleReference = true
            };

            var script = "Console.WriteLine('Started:' + new Date().toJSON()); " +
                         "var now = new Date().getTime(); " +
                         "Console.WriteLine('Finished:' + new Date().toJSON());";


            await manager.ExecuteAsync("test", script);

        }

        [Test]
        public async void Script_With_0_Timeout_Works()
        {
            var manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 })
            {
                AddConsoleReference = true
            };

            var script = "Console.WriteLine('Started:' + new Date().toJSON()); " +
                         "var now = new Date().getTime(); " +
                         "Console.WriteLine('Finished:' + new Date().toJSON());";


            await manager.ExecuteAsync("test", script);

        }

    }
}