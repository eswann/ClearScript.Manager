using System;
using System.Text;
using ClearScript.Manager.Caching;
using ClearScript.Manager.Loaders;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager
{
    internal class ScriptCompiler
    {
        private readonly LruCache<string, CachedV8Script> _scriptCache;
        private readonly IManagerSettings _settings;
        private readonly V8Runtime _v8Runtime;

        public ScriptCompiler(V8Runtime v8Runtime, IManagerSettings settings)
        {
            _settings = settings;
            _v8Runtime = v8Runtime;
            _scriptCache = new LruCache<string, CachedV8Script>(LurchTableOrder.Access, settings.ScriptCacheMaxCount);
        }

        public V8Script Compile(string scriptId, string code, bool addToCache = true, int? cacheExpirationSeconds = null)
        {
            CachedV8Script cachedScript;
            if (TryGetCached(scriptId, out cachedScript))
            {
                return cachedScript.Script;
            }

            V8Script compiledScript = _v8Runtime.Compile(scriptId, code);

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

        public V8Script Compile(IncludeScript script, bool addToCache = true, int? cacheExpirationSeconds = null)
        {
            CachedV8Script cachedScript;

            script.EnsureScriptId();

            if (TryGetCached(script.ScriptId, out cachedScript))
            {
                return cachedScript.Script;
            }

            if (string.IsNullOrEmpty(script.Code) && !string.IsNullOrEmpty(script.Uri))
            {
                script.LoadScript();
            }
            if (!string.IsNullOrEmpty(script.Code))
            {
                if (script.RequiredPackage != null && !string.IsNullOrEmpty(script.RequiredPackage.PackageId))
                {
                    script.Code = script.Code.Replace("this.exports", script.RequiredPackage.PackageId + ".exports");
                }
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

        public bool TryGetCached(string scriptId, out CachedV8Script cachedScript)
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