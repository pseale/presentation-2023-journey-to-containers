$ErrorActionPreference = "Stop"

# clear out bin/obj folders - we can't trust dotnet clean
Remove-Item "$PScriptRoot/bin" -Recurse -Force -ErrorAction 0
Remove-Item "$PScriptRoot/obj" -Recurse -Force -ErrorAction 0

cd $PSScriptRoot
dotnet publish -c Release

docker build $PSScriptRoot -t "hello-world:1.0.0.$(Get-Random -Minimum 1 -Maximum 999)"
