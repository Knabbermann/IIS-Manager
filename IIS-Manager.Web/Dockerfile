#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IIS-Manager.Web/IIS-Manager.Web.csproj", "IIS-Manager.Web/"]
COPY ["IIS-Manager.API/IIS-Manager.Controllers.csproj", "IIS-Manager.API/"]
COPY ["IIS-Manager.DataAccess/IIS-Manager.DataAccess.csproj", "IIS-Manager.DataAccess/"]
COPY ["IIS-Manager.Models/IIS-Manager.Models.csproj", "IIS-Manager.Models/"]
COPY ["IIS-Manager.Utility/IIS-Manager.Utility.csproj", "IIS-Manager.Utility/"]
RUN dotnet restore "./IIS-Manager.Web/IIS-Manager.Web.csproj"
COPY . .
WORKDIR "/src/IIS-Manager.Web"
RUN dotnet build "./IIS-Manager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IIS-Manager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IIS-Manager.Web.dll"]