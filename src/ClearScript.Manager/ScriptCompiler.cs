using System;
using System.Text;
using ClearScript.Manager.Caching;
using ClearScript.Manager.Loaders;
using JetBrains.Annotations;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager
{
    internal class ScriptCompiler
    {
        [NotNull]
        private readonly LruCache<string, CachedV8Script> _scriptCache;

        [NotNull]
        private readonly IManagerSettings _settings;

        [NotNull]
        private readonly V8Runtime _v8Runtime;

        public ScriptCompiler([NotNull] V8Runtime v8Runtime, [NotNull] IManagerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _v8Runtime = v8Runtime ?? throw new ArgumentNullException(nameof(v8Runtime));
            _scriptCache = new LruCache<string, CachedV8Script>(LurchTableOrder.Access, settings.ScriptCacheMaxCount);
        }

        [CanBeNull]
        public V8Script Compile(string scriptId, string code, bool addToCache = true, int? cacheExpirationSeconds = null)
        {
            if (TryGetCached(scriptId, out var cachedScript))
            {
                return cachedScript.Script;
            }

            var compiledScript = _v8Runtime.Compile(scriptId, code);

            if (addToCache)
            {
                if (!cacheExpirationSeconds.HasValue)
                {
                    cacheExpirationSeconds = _settings.ScriptCacheExpirationSeconds;
                }
                if (cacheExpirationSeconds > 0)
                {
                    var cacheEntry = new CachedV8Script(compiledScript, cacheExpirationSeconds.Value);
                    _scriptCache.AddOrUpdate(scriptId, cacheEntry, (key, original) => cacheEntry);
                }
            }

            return compiledScript;
        }

        [CanBeNull]
        public V8Script Compile([NotNull] IncludeScript script, bool addToCache = true, int? cacheExpirationSeconds = null)
        {
            if (script == null) throw new ArgumentNullException(nameof(script));

            script.EnsureScriptId();

            if (TryGetCached(script.ScriptId, out var cachedScript))
            {
                return cachedScript.Script;
            }

            if (string.IsNullOrEmpty(script.Code) && !string.IsNullOrEmpty(script.Uri))
            {
                script.LoadScript();
            }
            if (!string.IsNullOrEmpty(script.Code))
            {
                if (!string.IsNullOrEmpty(script.PrependCode) || !string.IsNullOrEmpty(script.AppendCode))
                {
                    var builder = new StringBuilder();
                    builder.Append(script.PrependCode).Append(script.Code).Append(script.AppendCode);
                    return Compile(script.ScriptId, builder.ToString(), addToCache, cacheExpirationSeconds); 
                }
                return Compile(script.ScriptId, script.Code, addToCache, cacheExpirationSeconds);
            }
            return null;
        }
 
        [ContractAnnotation("=> true, cachedScript:notnull;=> false, cachedScript:null")]
        public bool TryGetCached([CanBeNull] string scriptId, [CanBeNull] out CachedV8Script cachedScript)
        {
            if (_scriptCache.TryGetValue(scriptId, out cachedScript))
            {
                if (cachedScript.ExpiresOn > DateTime.UtcNow)
                {
                    cachedScript.CacheHits++;
                    return true;
                }
                _scriptCache.TryRemove(scriptId, out cachedScript);
            }
            cachedScript = null;
            return false;
        }

    }
}