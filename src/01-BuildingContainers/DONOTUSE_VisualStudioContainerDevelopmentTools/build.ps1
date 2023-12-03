$ErrorActionPreference = "Stop"

cd $PSScriptRoot
# `dotnet publish` is not necessary, because we build in the container!
# dotnet publish -c Release

docker build $PSScriptRoot -t "using-container-developer-tools:1.0.0.$(Get-Random -Minimum 1 -Maximum 999)"
