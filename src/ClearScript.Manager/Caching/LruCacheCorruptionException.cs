using System;

namespace ClearScript.Manager.Caching
{
    [Serializable]
    public class LruCacheCorruptionException : Exception
    {
        /// <inheritdoc />
        public override string Message => "LRU Cache is corrupted.";
    }
}