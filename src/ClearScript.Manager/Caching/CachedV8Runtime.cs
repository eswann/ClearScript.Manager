using System;
using JetBrains.Annotations;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager.Caching
{
    /// <summary>
    /// A cached instance of a compiled script including information on its creation and expiration.
    /// </summary>
    public class CachedV8Script
    {
        /// <summary>
        /// Creates a new <see cref="CachedV8Script"/> instance.
        /// </summary>
        /// <param name="script">The script that is being cached.</param>
        /// <param name="expirationSeconds">
        /// The number of seconds the script should be considered valid before 
        /// expiring.
        /// </param>
        public CachedV8Script([NotNull] V8Script script,
            int expirationSeconds = ManagerSettings.DefaultScriptTimeoutMilliSeconds)
        {
            CreatedOn = DateTime.UtcNow;
            ExpiresOn = CreatedOn.AddSeconds(expirationSeconds);
            Script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// The cached script instance.
        /// </summary>
        [NotNull]
        public V8Script Script { get; }

        /// <summary>
        /// The time the cached script was created.
        /// </summary>
        public DateTime CreatedOn { get; }

        /// <summary>
        /// The time the cached script should be considered expired.
        /// </summary>
        public DateTime ExpiresOn { get; }

        /// <summary>
        /// The number of times this cached script has been used.
        /// </summary>
        public int CacheHits { get; internal set; }
    }
}