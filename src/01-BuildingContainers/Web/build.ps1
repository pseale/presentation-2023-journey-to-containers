$ErrorActionPreference = "Stop"

cd $PSScriptRoot
# all-in-one: dotnet publish compiles, builds, publishes, and creates a docker container
dotnet publish --os linux --arch x64 -t:PublishContainer -c Release
