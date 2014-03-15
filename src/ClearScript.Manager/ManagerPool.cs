using System;
using System.Collections.Concurrent;

namespace ClearScript.Manager
{
    public interface IManagerPool
    {
        IRuntimeManager GetRuntime();
        void ReturnToPool(IRuntimeManager runtimeManager);
    }

    public class ManagerPool : IManagerPool
    {
        public static IManagerPool CurrentPool;

        private readonly object _addLock = new object();
        public int RuntimeCurrentCount = 0;

        private readonly BlockingCollection<IRuntimeManager> _availableRuntimes = new BlockingCollection<IRuntimeManager>();
        private readonly IManagerSettings _settings;

        public ManagerPool(IManagerSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException("settings", "Settings must be supplied to the RuntimePool.");

            _settings = settings;
        }

        public static void InitializeCurrentPool(IManagerSettings settings)
        {
            CurrentPool = new ManagerPool(settings);
        }

        public IRuntimeManager GetRuntime()
        {
            int runtimeMaxCount = _settings.RuntimeMaxCount;
            if (RuntimeCurrentCount < runtimeMaxCount)
            {
                lock (_addLock)
                {
                    if (RuntimeCurrentCount < runtimeMaxCount)
                    {
                        var runtime = new RuntimeManager(_settings);
                        RuntimeCurrentCount++;
                        return runtime;
                    }
                }
            }
            return _availableRuntimes.Take();
        }

        public void ReturnToPool(IRuntimeManager runtimeManager)
        {
            _availableRuntimes.Add(runtimeManager);
        }

    }
}