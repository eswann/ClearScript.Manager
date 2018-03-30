using System;

namespace JavaScript.Manager.Caching
{
    public class LruCacheCorruptionException : Exception
    {
        public override string Message
        {
            get { return "LRU Cache is corrupted."; }
        }
    }
}