using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace ClearScript.Manager
{
    /// <summary>
    /// Defines a pool from which Runtime Managers are requested.
    /// </summary>
    public interface IManagerPool
    {
        /// <summary>
        /// Gets a runtime from the pool.  Blocks until a manager becomes available.
        /// </summary>
        /// <returns>The next available <see cref="IRuntimeManager"/>.</returns>
        [NotNull]
        IRuntimeManager GetRuntime();

        /// <summary>
        /// Returns a Runtime Manager to the pool.
        /// </summary>
        /// <param name="runtimeManager">The Runtime Manager to return to the pool.</param>
        void ReturnToPool([NotNull] IRuntimeManager runtimeManager);
    }

    /// <inheritdoc />
    /// <summary>
    /// Defines a pool from which Runtime Managers are requested.
    /// </summary>
    public class ManagerPool : IManagerPool
    {
        /// <summary>
        /// Static reference to the current pool of Runtime Managers.
        /// </summary>
        [CanBeNull]
        public static IManagerPool CurrentPool;

        private readonly object _addLock = new object();

        /// <summary>
        /// Current count of Runtimes within the pool.
        /// </summary>
        public int RuntimeCurrentCount { get; private set; }

        private readonly BlockingCollection<IRuntimeManager> _availableRuntimes = new BlockingCollection<IRuntimeManager>();
        private readonly IManagerSettings _settings;

        /// <summary>
        /// Creates a new Runtime Manager Pool.
        /// </summary>
        /// <param name="settings">Settings to apply to the Runtime Manager.</param>
        public ManagerPool([NotNull] IManagerSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings), "Settings must be supplied to the RuntimePool.");

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

        /// <inheritdoc />
        public IRuntimeManager GetRuntime()
        {
            if (_availableRuntimes.TryTake(out var manager))
                return manager;
            
            var runtimeMaxCount = _settings.RuntimeMaxCount;
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

        /// <inheritdoc />
        public void ReturnToPool(IRuntimeManager runtimeManager)
        {
            if (runtimeManager == null) throw new ArgumentNullException(nameof(runtimeManager));
            _availableRuntimes.Add(runtimeManager);
        }

    }
}