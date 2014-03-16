using System.Threading;
using ClearScript.Manager.Caching;
using NUnit.Framework;
using Should;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class WhenCachingScripts
    {
        [Test]
        public async void Compiled_Script_Is_Cached_By_Default()
        {
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("test", "var i = 0; i++;");

            CachedV8Script cached;
            manager.TryGetCached("test", out cached).ShouldBeTrue();
        }

        [Test]
        public async void Cached_Script_Shows_Cache_Hits()
        {
            var manager = new RuntimeManager(new ManualManagerSettings());

            await manager.ExecuteAsync("test", "var i = 0; i++;");

            await manager.ExecuteAsync("test", "var i = 0; i++;");

            CachedV8Script cached;
            manager.TryGetCached("test", out cached);

            cached.CacheHits.ShouldBeGreaterThan(1);
        }

        [Test]
        public void Zero_Expiration_Script_Is_Not_Cached()
        {
            var manager = new RuntimeManager(new ManualManagerSettings());

            manager.Compile("test", "var i = 0; i++;", true, 0);

            CachedV8Script cached;
            manager.TryGetCached("test", out cached).ShouldBeFalse();
        }

        [Test]
        public void Uncached_Script_Is_Not_Cached()
        {
            var manager = new RuntimeManager(new ManualManagerSettings());

            manager.Compile("test", "var i = 0; i++;", false);

            CachedV8Script cached;
            manager.TryGetCached("test", out cached).ShouldBeFalse();
        }

        [Test]
        public void Expired_Script_Is_Not_Returned()
        {
            var manager = new RuntimeManager(new ManualManagerSettings());

            manager.Compile("test", "var i = 0; i++;", true, 1);

            Thread.Sleep(1000);

            CachedV8Script cached;
            manager.TryGetCached("test", out cached).ShouldBeFalse();
        }


    }
}