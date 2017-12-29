using System;
using System.Threading.Tasks;
using JavaScript.Manager;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenUsingAManagerPool
    {
        [Test]
        public async void Script_Exceeding_Pool_Count_Blocks_Until_Item_Available()
        {

            //Set the manager max count
            ManagerPool.InitializeCurrentPool(new ManualManagerSettings{ RuntimeMaxCount = 2 });

            const string script = "Console.WriteLine('Started {0}:' + new Date().toJSON()); " +
                                  "var now = new Date().getTime(); while(new Date().getTime() < now + 1000){{ /* do nothing */ }}; " +
                                  "Console.WriteLine('finished {0}:' + new Date().toJSON());";

            var startDate = DateTime.UtcNow;

            await Task.WhenAll(
                RunInScope("first", script),
                RunInScope("second", script),
                RunInScope("third", script));

            DateTime.UtcNow.Subtract(startDate).ShouldBeGreaterThan(new TimeSpan(0, 0, 2));
        }

        private static Task RunInScope(string name, string script)
        {
            return Task.Run(async () =>
            {
                using (var scope = new ManagerScope())
                {
                    scope.RuntimeManager.AddConsoleReference = true;
                    await scope.RuntimeManager.ExecuteAsync(name, script);
                }
            });
        }


    }
}