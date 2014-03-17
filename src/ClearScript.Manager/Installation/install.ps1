param($installPath, $toolsPath, $package, $project)

$project.ProjectItems.Item("Lib").ProjectItems.Item("ClearScriptV8-32.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("Lib").ProjectItems.Item("ClearScriptV8-32.pdb").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("Lib").ProjectItems.Item("ClearScriptV8-64.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("Lib").ProjectItems.Item("ClearScriptV8-64.pdb").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("Lib").ProjectItems.Item("v8-ia32.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("Lib").ProjectItems.Item("v8-ia32.pdb").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("Lib").ProjectItems.Item("v8-x64.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("Lib").ProjectItems.Item("v8-x64.pdb").Properties.Item("CopyToOutputDirectory").Value = 1