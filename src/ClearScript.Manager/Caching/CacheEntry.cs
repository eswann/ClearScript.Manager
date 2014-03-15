using System;

namespace ClearScript.Manager.Caching
{
    public class CacheEntry<TValue>
    {
        public CacheEntry ()
        {
        }

        public CacheEntry(TValue entry)
        {
            CreateDate = DateTime.UtcNow;
            Entry = entry;
        }
        
        public TValue Entry { get; set; }

        public DateTime CreateDate { get; set; }
    }
}