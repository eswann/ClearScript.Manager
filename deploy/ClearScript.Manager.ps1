properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\src\ClearScript.Manager.sln"
	$ProjectPath = "$BaseDir\src\ClearScript.Manager\ClearScript.Manager.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "ClearScript.Manager"
}

. .\common.ps1
