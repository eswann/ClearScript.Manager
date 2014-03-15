using System;

namespace ClearScript.Manager
{
    public class ManagerScope : IDisposable
    {
        private bool _disposed;

        public ManagerScope()
        {
            RuntimeManager = ManagerPool.CurrentPool.GetRuntime();
        }

        public IRuntimeManager RuntimeManager { get; private set; }

        public void Dispose()
        {
            if (!_disposed)
            {
               ManagerPool.CurrentPool.ReturnToPool(RuntimeManager);
                _disposed = true;
            }
        }
    }
}