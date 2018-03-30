using System;
using System.Collections.Concurrent;

namespace JavaScript.Manager
{
    /// <summary>
    /// Defines a pool from which Runtime Managers are requested.
    /// </summary>
    public interface IManagerPool
    {
        /// <summary>
        /// Gets a runtime from the pool.  Blocks until a manager becomes available.
        /// </summary>
        /// <returns>The next available IRuntimeManager.</returns>
        IRuntimeManager GetRuntime();

        /// <summary>
        /// Returns a Runtime Manager to the pool.
        /// </summary>
        /// <param name="runtimeManager">The Runtime Manager to return to the pool.</param>
        void ReturnToPool(IRuntimeManager runtimeManager);
    }

    /// <summary>
    /// Defines a pool from which Runtime Managers are requested.
    /// </summary>
    public class ManagerPool : IManagerPool
    {
        /// <summary>
        /// Static reference to the current pool of Runtime Managers.
        /// </summary>
        public static IManagerPool CurrentPool;

        private readonly object _addLock = new object();

        /// <summary>
        /// Current count of Runtimes within the pool.
        /// </summary>
        public int RuntimeCurrentCount = 0;

        private readonly BlockingCollection<IRuntimeManager> _availableRuntimes = new BlockingCollection<IRuntimeManager>();
        private readonly IManagerSettings _settings;

        /// <summary>
        /// Creates a new Runtime Manager Pool.
        /// </summary>
        /// <param name="settings">Settings to apply to the Runtime Manager.</param>
        public ManagerPool(IManagerSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException("settings", "Settings must be supplied to the RuntimePool.");

            _settings = settings;
        }

        /// <summary>
        /// Initializes the CurrentPool with the provided settings.  
        /// </summary>
        /// <param name="settings">Settings to apply.</param>
        public static void InitializeCurrentPool(IManagerSettings settings)
        {
            CurrentPool = new ManagerPool(settings);
        }

        public IRuntimeManager GetRuntime()
        {
            IRuntimeManager manager;
            if (_availableRuntimes.TryTake(out manager))
                return manager;
            
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