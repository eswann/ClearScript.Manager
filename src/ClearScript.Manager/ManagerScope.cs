using System;
using JetBrains.Annotations;

namespace ClearScript.Manager
{
    /// <summary>
    /// Scope in which a Runtime Manager is requested from the pool and returned upon completion.
    /// </summary>
    public class ManagerScope : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Creates a new ManagerScope and requests a RuntimeManager.
        /// </summary>
        public ManagerScope()
        {
            if (ManagerPool.CurrentPool == null)
                throw new InvalidOperationException("ManagerPool is not initialized.");

            RuntimeManager = ManagerPool.CurrentPool.GetRuntime();
        }

        /// <summary>
        /// Allows access to the allocated Runtime Manager from within this scope.
        /// </summary>
        [NotNull]
        public IRuntimeManager RuntimeManager { get; }

        /// <summary>
        /// Disposed the object and returns the Runtime Manager to the pool.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                RuntimeManager.Cleanup();

                if (ManagerPool.CurrentPool == null)
                    throw new InvalidOperationException("ManagerPool is not initialized.");

                ManagerPool.CurrentPool.ReturnToPool(RuntimeManager);
                _disposed = true;
            }
        }
    }
}