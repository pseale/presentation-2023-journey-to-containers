$ErrorActionPreference = "Stop"

# clear out bin/obj folders - we can't trust dotnet clean
Remove-Item "$PScriptRoot/bin" -Recurse -Force -ErrorAction 0
Remove-Item "$PScriptRoot/obj" -Recurse -Force -ErrorAction 0

cd $PSScriptRoot
# all-in-one: dotnet publish compiles, builds, publishes, and creates a docker container
dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
