# ClearScript.Manager

[![Build status](https://ci.appveyor.com/api/projects/status/b43oj4a3ccj7fm31?svg=true)](https://ci.appveyor.com/project/eswann/clearscript-manager)

ClearScript Manager was created to encapsulate the use of the ClearScript V8 engine in multi-use scenarios, like in a hosted server project (Ex: for use in a Web App or Web API).

ClearScript is an awesome library that was created to allow execution of JavaScript via V8 from the .Net runtime. 
It's a great project but it needed a few extra things in certain situations that aren't in the core goals of ClearScript itself.
ClearScript also runs VBScript and JScript but those are not in the scope of ClearScript.Manager at the current time.

It should be noted that the package also installs the latest version of ClearScript (5.4) via ClearScript.Installer.
In addition, both ClearScript and ClearScript.Manager are compiled against .Net 4.5.

Here are a couple of the related discussions on the clearscript forum:

<https://clearscript.codeplex.com/discussions/535693>  
<https://clearscript.codeplex.com/discussions/533516>  

And the ClearScript site: https://clearscript.codeplex.com

Along those lines, ClearScript.Manager does the following to make certain things easier in your server project:

* Downloads and adds the ClearScript dlls appropriately.
* Creates a configurable pool of V8 Runtimes that are cached and reused.
	- Pools have a configurable number of max instances.
	- Behavior when attempting to retrieve a V8 Runtime is to block until a V8 engine becomes available.
* Because V8 Runtimes have affinity for compiled scripts, it compiles and caches scripts for each V8 Runtime instance.
* Attempts to better contain running V8 scripts by:
	- Setting up a Task in which the script is run with a configurable timeout.
	- Allow easy management of the memory usage of each instance of the V8 Runtime and sets the limits to a much lower threshold than the default V8 settings. 

## Configuration Parameters
These settings can be set manually or added to the AppSettings of your application config file.
  
    //V8 Max Executable Size in bytes:  
    int MaxExecutableBytes { get; }

    //V8 Max New Space in bytes:
    int MaxNewSpaceBytes { get; }
     
    //V8 Max Old Space in bytes:  
	int MaxOldSpaceBytes { get; }
        
	//Default script timeout in ms.  
    //If the script timeout is set to 0, the script will not execute within a Task with a timeout.
	int ScriptTimeoutMilliSeconds { get; }  
    
    //Max number of simultaneous V8 Runtimes:  
    int RuntimeMaxCount { get; }
    
    //Per Runtime, the maximum number of cached scripts:  
    int ScriptCacheMaxCount { get; }

	//The default script cache expiration in seconds:  
	int ScriptCacheExpirationSeconds { get; }

    //Is V8 debugging enabled
    bool V8DebugEnabled {get;}

    //What is the V8 Debug port to connect on.  Default is 9222.
    bool V8DebugPort {get;}


## Using the Runtime Manager

### Running Basic JavaScript
Grab a runtime manager and use the ExecuteAsync method. 

    var manager = new RuntimeManager(new ManagerSettings());  
    await manager.ExecuteAsync("test", "var i = 0; i++;");

### Running Multiple Scripts
To run multiple scripts, use the ExecuteAsync that accepts an enumerable of scripts.  Scripts will be run in order.  
Settings will be applied to all scripts in the collection.  Scripts can be actual code, a local file path or a Uri.

    await manager.ExecuteAsync(new List<IncludeScript>
    {
        new IncludeScript {Uri = ".\\TestMainScript.js", ScriptId = "testScript"},
        new IncludeScript {Code = "subject.TestString = 'test string3';", ScriptId = "testScript3"},
        new IncludeScript {Uri = "https://www.myscriptishere.org/script.txt", ScriptId = "TestScript4"}
    });
    

### Execution Options
Execution options are the new way of passing options to the ClearScript runtime manager.

    /// Objects to inject into the JavaScript runtime.
    public IEnumerable<HostObject> HostObjects

    /// Types to make available to the JavaScript runtime.
    public IEnumerable<HostType> HostTypes

    /// Indicates that this script should be added to the script cache once compiled.  
    /// Default is True.
    public bool AddToCache

    /// External JavaScripts to import before executing the current script.
    public IList<IncludeScript> Scripts

#### Passing in Host Objects
Host objects are a way of passing an object instance to JavaScript from .Net.

    var subject = new TestObject{Name = "Name", Count = 0};
    var manager = new RuntimeManager(new ManualManagerSettings());

    await manager.ExecuteAsync("testscript", "subject.Count = 10;",
        new ExecutionOptions{HostObjects = 
            new List<HostObject> {new HostObject {Name = "subject", Target = subject}}});

#### Passing in Host Types
Host types allow you to instantiate a .Net type or types in the JavaScript runtime. 

    var manager = new RuntimeManager(new ManagerSettings());
    var hostType = new HostType
    {
        Name = "MathStuff",
        Type = typeof(System.Math)
    };
    var subject = new TestObject();

    await manager.ExecuteAsync("testscript", "subject.Result = MathStuff.Pow(10,2);", 
        new ExecutionOptions{
        HostObjects = new List<HostObject> { new HostObject { Name = "subject", Target = subject } }, 
        HostTypes = new List<HostType> { hostType }});

#### Passing in Included Scripts
Included scripts can also be run by setting the IncludeScripts property on the ExecutionOptions.  These scripts are 
intended to set up reused libraries and will be run before the execution of the main script.  

A script can be set up in a couple of ways:
* Set a file path or url in the Uri property.
* Set the code property to the code to execute.

The script will be compiled and cached in the same way as a normal script.

    public class IncludeScript
    {
        /// Unique name of the script to execute.
        public string Name { get; set; }

        /// Uri (file or Url) of the script to execute.  Need to include script code or script Url.
        public string Uri { get; set; }

        /// Code of the script to include.  Need to include script code or script Url.
        public string Code { get; set; }
    }
    

### Using the Manager Pool

#### Initialize the Manager Pool

	//ManagerSettings should contain the maximum pool count
	ManagerPool.InitializeCurrentPool(new ManagerSettings());

#### Using the Manager Scope to automatically Get/Return a Runtime Manager

    using (var scope = new ManagerScope())
	{
		await scope.RuntimeManager.ExecuteAsync(name, "var i = 0; i++;");
	}

#### Getting and Returning a Runtime Manager

	var runtimeManager = ManagerPool.CurrentPool.GetRuntime()
	ManagerPool.CurrentPool.ReturnToPool(runtimeManager);


Additional examples are available in the unit tests.