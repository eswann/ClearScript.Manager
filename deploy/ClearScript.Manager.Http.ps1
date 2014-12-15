properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\src\ClearScript.Manager.sln"
	$ProjectPath = "$BaseDir\src\ClearScript.Manager.Http\ClearScript.Manager.Http.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "ClearScript.Manager.Http"
}

. .\common.ps1
