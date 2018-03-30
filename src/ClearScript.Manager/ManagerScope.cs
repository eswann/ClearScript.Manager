﻿using JavaScript.Manager.Loaders;
using System;

namespace JavaScript.Manager
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
            RuntimeManager = ManagerPool.CurrentPool.GetRuntime();
        }

        public ManagerScope(RequireManager requireManager)
        {
            RuntimeManager = ManagerPool.CurrentPool.GetRuntime();
            RuntimeManager.RequireManager = requireManager;
        }

        /// <summary>
        /// Allows access to the allocated Runtime Manager from within this scope.
        /// </summary>
        public IRuntimeManager RuntimeManager { get; private set; }

        /// <summary>
        /// Disposed the object and returns the Runtime Manager to the pool.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                RuntimeManager.Cleanup();
                RuntimeManager.RequireManager?.ClearPackages();
                ManagerPool.CurrentPool.ReturnToPool(RuntimeManager);
                _disposed = true;
            }
        }
    }
}