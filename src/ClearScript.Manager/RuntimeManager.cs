using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClearScript.Manager.Caching;
using ClearScript.Manager.Extensions;
using ClearScript.Manager.Loaders;
using JetBrains.Annotations;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager
{
    /// <summary>
    /// Runtime Manager used to execute scripts within a runtime.
    /// </summary>
    public interface IRuntimeManager : IDisposable
    {
        /// <summary>
        /// If True, automatically adds a reference to the .Net Console.
        /// </summary>
        bool AddConsoleReference { get; set; }

        /// <summary>
        /// Executes the provided script.
        /// </summary>
        /// <param name="scriptId">Id of the script for caching purposes.</param>
        /// <param name="code">Script text to execute.</param>
        /// <param name="configAction">An action that accepts the V8 script engine before it's use and configures it.</param>
        /// <param name="addToCache">Indicates that this script should be added to the script cache once compiled.  Default is True.</param>
        /// <returns>Task to await.</returns>
        [Obsolete("Obsolete, use the overload accepting ExecutionOptions instead.")]
        Task ExecuteAsync(string scriptId, string code, Action<V8ScriptEngine> configAction, bool addToCache = true);

        /// <summary>
        /// Executes the provided script.
        /// </summary>
        /// <param name="scriptId">Id of the script for caching purposes.</param>
        /// <param name="code">Script text to execute.</param>
        /// <param name="addToCache">Indicates that this script should be added to the script cache once compiled.  Default is True.</param>
        /// <param name="hostObjects">Objects to inject into the JavaScript runtime.</param>
        /// <param name="hostTypes">Types to make available to the JavaScript runtime.</param>
        /// <returns>Task to await.</returns>
        [Obsolete("Obsolete, use the overload accepting ExecutionOptions instead.")]
        Task ExecuteAsync(string scriptId, string code, [CanBeNull] IList<HostObject> hostObjects, [CanBeNull] IList<HostType> hostTypes = null, bool addToCache = true);

        /// <summary>
        /// Executes the provided script.
        /// </summary>
        /// <param name="scripts">A list of include scripts to run.</param>
        /// <param name="configAction">An action that accepts the V8 script engine before it's use and configures it.</param>
        /// <param name="options">Options to apply to script execution.  HostObjects and HostTypes are ignored by this call.</param>
        /// <returns>Task to await.</returns>
        Task<V8ScriptEngine> ExecuteAsync([CanBeNull] IEnumerable<IncludeScript> scripts,
            [CanBeNull] Action<V8ScriptEngine> configAction, [CanBeNull] ExecutionOptions options = null);

        /// <summary>
        /// Executes the provided script.
        /// </summary>
        /// <param name="scripts">A list of include scripts to run.</param>
        /// <param name="options">Options to apply to script execution.  HostObjects and HostTypes are ignored by this call.</param>
        /// <returns>Task to await.</returns>
        Task<V8ScriptEngine> ExecuteAsync([CanBeNull] IEnumerable<IncludeScript> scripts, [CanBeNull] ExecutionOptions options = null);

        /// <summary>
        /// Executes the provided script.
        /// </summary>
        /// <param name="scriptId">Id of the script for caching purposes.</param>
        /// <param name="code">Script text to execute.</param>
        /// <param name="configAction">An action that accepts the V8 script engine before it's use and configures it.</param>
        /// <param name="options">Options to apply to script execution.  HostObjects and HostTypes are ignored by this call.</param>
        /// <returns>Task to await.</returns>
        Task<V8ScriptEngine> ExecuteAsync(string scriptId, string code, Action<V8ScriptEngine> configAction, ExecutionOptions options = null);

        /// <summary>
        /// Executes the provided script.
        /// </summary>
        /// <param name="scriptId">Id of the script for caching purposes.</param>
        /// <param name="code">Script text to execute.</param>
        /// <param name="options">Options to apply to script execution.  HostObjects and HostTypes are ignored by this call.</param>
        /// <returns>Task to await.</returns>
        Task<V8ScriptEngine> ExecuteAsync(string scriptId, string code, ExecutionOptions options = null);

        /// <summary>
        /// Compiles the provided script.
        /// </summary>
        /// <param name="scriptId">Id of the script for caching purposes.</param>
        /// <param name="code">Script text to execute.</param>
        /// <param name="addToCache">Indicates that this script should be added to the script cache once compiled.  Default is True.</param>
        /// <param name="cacheExpirationSeconds">Number of seconds that the V8 Script will be cached.</param>
        /// <returns>Compiled V8 script.</returns>
        V8Script Compile(string scriptId, string code, bool addToCache = true, int? cacheExpirationSeconds = null);

        /// <summary>
        /// Attempts to get the cached script by Id.
        /// </summary>
        /// <param name="scriptId">Id to search on.</param>
        /// <param name="script">Cached script (output)</param>
        /// <returns>Bool indicating that cached script was located.</returns>
        bool TryGetCached(string scriptId, out CachedV8Script script);

        /// <summary>
        /// Retrieves the script engine for the current runtime manager.
        /// </summary>
        /// <returns></returns>
        V8ScriptEngine GetEngine();

        /// <summary>
        /// Cleans up resources in the current runtime.
        /// </summary>
        void Cleanup();
    }


    /// <summary>
    /// Runtime Manager used to execute scripts within a runtime.
    /// </summary>
    public class RuntimeManager : IRuntimeManager
    {
        private readonly IManagerSettings _settings;

        [NotNull]
        private readonly V8Runtime _v8Runtime;

        [NotNull]
        private readonly ScriptCompiler _scriptCompiler;

        [CanBeNull]
        private V8ScriptEngine _scriptEngine;

        private bool _disposed;

        /// <summary>
        /// Creates a new Runtime Manager.
        /// </summary>
        /// <param name="settings">Settings to apply to the Runtime Manager and the scripts it runs.</param>
        public RuntimeManager(IManagerSettings settings)
        {
            _settings = settings;

            _v8Runtime = new V8Runtime(new V8RuntimeConstraints
            {
                MaxExecutableSize = settings.MaxExecutableBytes,
                MaxOldSpaceSize = settings.MaxOldSpaceBytes,
                MaxNewSpaceSize = settings.MaxNewSpaceBytes
            });

            _scriptCompiler = new ScriptCompiler(_v8Runtime, settings);
        }

        /// <inheritdoc />
        public bool AddConsoleReference { get; set; }

        /// <inheritdoc />
        [Obsolete("Obsolete, use the overload accepting ExecutionOptions instead.")]
        public async Task ExecuteAsync(string scriptId, string code, Action<V8ScriptEngine> configAction, bool addToCache = true)
        {
            await ExecuteAsync(scriptId, code, configAction, new ExecutionOptions {AddToCache = addToCache});
        }

        /// <inheritdoc />
        public async Task<V8ScriptEngine> ExecuteAsync(string scriptId, string code, Action<V8ScriptEngine> configAction, ExecutionOptions options = null)
        {
            return await ExecuteAsync(new[] {new IncludeScript {Code = code, ScriptId = scriptId}}, configAction, options);
        }

        /// <inheritdoc />
        public async Task<V8ScriptEngine> ExecuteAsync(IEnumerable<IncludeScript> scripts, Action<V8ScriptEngine> configAction, ExecutionOptions options = null)
        {
            var scriptList = PrecheckScripts(scripts);
            if (scriptList == null)
                return null;

            if (options == null)
                options = new ExecutionOptions();

            var compiledScripts = scriptList.Select(x => _scriptCompiler.Compile(x, options.AddToCache, options.CacheExpirationSeconds));

            var scriptEngine = GetEngine();

            if (AddConsoleReference)
            {
                scriptEngine.AddHostType("Console", typeof (Console));
            }

            RequireManager.BuildRequirer(_scriptCompiler, scriptEngine);

            configAction?.Invoke(scriptEngine);

            if (options.Scripts != null)
            {
                foreach (var script in options.Scripts)
                {
                    var compiledInclude = _scriptCompiler.Compile(script, options.AddToCache);
                    if (compiledInclude != null)
                    {
                        scriptEngine.Execute(compiledInclude);
                    }
                }
            }

            foreach (var compiledScript in compiledScripts)
            {
                //Only create a wrapping task if the script has a timeout.
                if (TryCreateCancellationToken(out var cancellationToken))
                {
                    using (cancellationToken.Register(scriptEngine.Interrupt))
                    {
                        try
                        {
                            var script = compiledScript;
                            await Task.Run(() => scriptEngine.Execute(script), cancellationToken).ConfigureAwait(false);
                        }
                        catch (ScriptInterruptedException ex)
                        {
                            var newEx = new ScriptInterruptedException(
                                "Script interruption occurred, this often indicates a script timeout.  Examine the data and inner exception for more information.", ex);
                            newEx.Data.Add("Timeout", _settings.ScriptTimeoutMilliSeconds);
                            newEx.Data.Add("ScriptId", compiledScript.Name);

                            throw newEx;
                        }
                    }
                }
                else
                {
                    scriptEngine.Execute(compiledScript);
                }
            }

            return scriptEngine;
        }

        /// <inheritdoc />
        [Obsolete("Obsolete, use the overload accepting ExecutionOptions instead.")]
        public async Task ExecuteAsync(string scriptId, string code, IList<HostObject> hostObjects, IList<HostType> hostTypes = null, bool addToCache = true)
        {
            await ExecuteAsync(scriptId, code,
                    new ExecutionOptions {HostObjects = hostObjects, HostTypes = hostTypes, AddToCache = addToCache});
        }

        /// <inheritdoc />
        public async Task<V8ScriptEngine> ExecuteAsync(string scriptId, string code, ExecutionOptions options = null)
        {
            return await ExecuteAsync(new[] {new IncludeScript {ScriptId = scriptId, Code = code}}, options);
        }

        /// <inheritdoc />
        public async Task<V8ScriptEngine> ExecuteAsync(IEnumerable<IncludeScript> scripts, ExecutionOptions options = null)
        {
            var scriptList = PrecheckScripts(scripts);
            if (scriptList == null)
                return null;

            foreach (var includeScript in scriptList)
            {
                if (string.IsNullOrEmpty(includeScript.ScriptId) && !string.IsNullOrEmpty(includeScript.Code))
                {
                    includeScript.ScriptId = includeScript.Code.GetHashCode().ToString(CultureInfo.InvariantCulture);
                }
            }

            if (options == null)
                options = new ExecutionOptions();

            var configAction = new Action<V8ScriptEngine>(engine => engine.ApplyOptions(options));

            return await ExecuteAsync(scriptList, configAction, options);
        }

        /// <inheritdoc />
        [NotNull]
        public V8ScriptEngine GetEngine()
        {
            if (_scriptEngine == null)
            {
                var flags = _settings.V8DebugEnabled
                    ? V8ScriptEngineFlags.DisableGlobalMembers | V8ScriptEngineFlags.EnableDebugging
                    : V8ScriptEngineFlags.DisableGlobalMembers;

                _scriptEngine = _v8Runtime.CreateScriptEngine(flags, _settings.V8DebugPort);

                if (_scriptEngine == null)
                    throw new InvalidOperationException("Failed to create v8 script engine");
            }
            return _scriptEngine;
        }

        /// <inheritdoc />
        public V8Script Compile(string scriptId, string code, bool addToCache = true, int? cacheExpirationSeconds = null)
        {
            return _scriptCompiler.Compile(scriptId, code, addToCache, cacheExpirationSeconds);
        }

        /// <inheritdoc />
        public bool TryGetCached(string scriptId, out CachedV8Script script)
        {
            return _scriptCompiler.TryGetCached(scriptId, out script);
        }

        /// <inheritdoc />
        public void Cleanup()
        {
            if (_scriptEngine != null)
            {
                _scriptEngine.Dispose();
                _scriptEngine = null;
            }
        }

        private bool TryCreateCancellationToken(out CancellationToken token)
        {
            if (_settings.ScriptTimeoutMilliSeconds <= 0)
            {
                token = new CancellationToken();
                return false;
            }

            var cancellationSource = new CancellationTokenSource(_settings.ScriptTimeoutMilliSeconds);
            token = cancellationSource.Token;
            token.ThrowIfCancellationRequested();

            return true;
        }

        [CanBeNull]
        private static IList<IncludeScript> PrecheckScripts([CanBeNull] IEnumerable<IncludeScript> scripts)
        {
            if (scripts == null)
                return null;

            var scriptList = scripts.ToList();

            return scriptList.Count == 0 ? null : scriptList;
        }

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true); 
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing the current object.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    Cleanup();
                }
            }
        }

        /// <inheritdoc />
        ~RuntimeManager()
        {
            Dispose(false);
        }

        #endregion

    }
}