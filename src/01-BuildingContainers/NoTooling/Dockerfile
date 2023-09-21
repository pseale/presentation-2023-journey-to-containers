# self-contained dotnet apps require the dotnet "runtime dependencies" base image
# can't use SCRATCH base image without extra work! (Don't use SCRATCH)
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0

# ASSUMING we have previously run `dotnet publish` - we will find a single binary
#(and perhaps a PDB) in the ./bin/Debug/net7.0/linux-x64/publish directory
COPY ./bin/Release/net7.0/linux-x64/publish ./app

ENTRYPOINT ["/app/01_ContainerHelloWorld"]