ClearScript.Manager
===================

ClearScript Manager was created to encapsulate the use of the ClearScript V8 engine in multi-use scenarios, like in a hosted server project (Ex: for use in a Web App or Web API).

ClearScript is an awesome library that was created to allow execution of JavaScript via V8 from the .Net runtime. 
It's an awesome project but it needed a few extra things in certain situations that aren't in the core goals of ClearScript itself.
ClearScript also runs VBScript and JScript but those are not in the scope of ClearScript.Manager at the current time.

Here are a couple of the related discussions on the clearscript forum:

<https://clearscript.codeplex.com/discussions/535693>  
<https://clearscript.codeplex.com/discussions/533516>  

Along those lines, ClearScript.Manager does the following to make certain things easier in your server project:

* Downloads and adds the ClearScript dlls appropriately.
* Creates a configurable pool of V8 Runtimes that are cached and reused.
	- Pools have a configurable number of max instances.
	- Behavior when attempting to retrieve a V8 Runtime is to block until a V8 engine becomes available.
* Because V8 Runtimes have affinity for compiled scripts, it compiles and caches scripts for each V8 Runtime instance.
* Attempts to better contain running V8 scripts by:
	- Setting up a Task in which the script is run with a configurable timeout.
	- Allow easy management of the memory usage of each instance of the V8 Runtime and sets the limits to a much lower threshold than the default V8 settings. 

Configuration Parameters
------------------------
  
V8 Max Executable Size in bytes:  
int MaxExecutableBytes { get; }
        
V8 Max Young Space in bytes:  
int MaxYoungSpaceBytes { get; }
        
V8 Max Old Space in bytes:  
int MaxOldSpaceBytes { get; }
        
Default script timeout in ms:
int ScriptTimeoutMilliSeconds { get; }  

Max number of simultaneous V8 Runtimes:  
int RuntimeMaxCount { get; }
        
Per Runtime, the maximum number of cached scripts:  
int ScriptCacheMaxCount { get; }
        
The default script cache expiration in seconds:  
int ScriptCacheExpirationSeconds { get; }



Examples are available in the unit tests...but I will add some simple guidance soon.