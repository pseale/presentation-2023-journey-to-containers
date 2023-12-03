$ErrorActionPreference = "Stop"

cd $PSScriptRoot
dotnet publish -c Release --os linux --arch x64
if (!$?) { throw "error during dotnet publish" }

docker build $PSScriptRoot -t "hello-world:1.0.0.$(Get-Random -Minimum 1 -Maximum 999)"
