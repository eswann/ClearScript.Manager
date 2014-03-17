@echo off

powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\archive.ps1; exit $error.Count}"


powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\package.ps1 -PackageName 'ClearScript.Manager'; exit $error.Count}"



pause