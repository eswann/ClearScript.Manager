using System;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager.Caching
{
    public class CachedV8Script
    {
        public CachedV8Script ()
        {
        }

        public CachedV8Script(V8Script script, int expirationSeconds = ManagerSettings.DefaultScriptTimeoutMilliSeconds)
        {
            CreatedOn = DateTime.UtcNow;
            ExpiresOn = CreatedOn.AddSeconds(expirationSeconds);
            Script = script;
        }

        public V8Script Script { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ExpiresOn { get; set; }

        public int CacheHits { get; internal set; }
    }
}