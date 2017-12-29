using JavaScript.Manager;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenUsingEngine
    {
        [Test]
        public void Script_Engine_Is_Retrieved()
        {
            ManagerPool.InitializeCurrentPool(new ManualManagerSettings { RuntimeMaxCount = 2 });

            using (var scope = new ManagerScope())
            {
                var engine = scope.RuntimeManager.GetEngine();

                engine.ShouldNotBeNull();

                engine.Execute("var i = 200;");

                Assert.AreEqual(200, engine.Script.i);
            }

        } 
    }
}