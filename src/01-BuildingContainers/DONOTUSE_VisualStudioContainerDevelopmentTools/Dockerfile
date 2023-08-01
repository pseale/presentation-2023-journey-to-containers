##########################################################
#
# NOTE FROM PSEALE 2023-08-09: do not use this Dockerfile!
# Not recommended! Also probably avoid 'Container
# developer tools' entirely
#
##########################################################

#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./DONOTUSE_VisualStudioContainerDevelopmentTools.csproj", "DONOTUSE_VisualStudioContainerDevelopmentTools/"]
RUN dotnet restore "DONOTUSE_VisualStudioContainerDevelopmentTools/DONOTUSE_VisualStudioContainerDevelopmentTools.csproj"
COPY . DONOTUSE_VisualStudioContainerDevelopmentTools/
WORKDIR "/src/DONOTUSE_VisualStudioContainerDevelopmentTools"
RUN dotnet build "DONOTUSE_VisualStudioContainerDevelopmentTools.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DONOTUSE_VisualStudioContainerDevelopmentTools.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DONOTUSE_VisualStudioContainerDevelopmentTools.dll"]