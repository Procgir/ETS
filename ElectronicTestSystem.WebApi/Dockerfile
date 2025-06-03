FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElectronicTestSystem.WebApi/ElectronicTestSystem.WebApi.csproj", "ElectronicTestSystem.WebApi/"]
RUN dotnet restore "ElectronicTestSystem.WebApi/ElectronicTestSystem.WebApi.csproj"
COPY . .
WORKDIR "/src/ElectronicTestSystem.WebApi"
RUN dotnet build "ElectronicTestSystem.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ElectronicTestSystem.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY wait-for-it.sh ./wait-for-it.sh
RUN chmod +x ./wait-for-it.sh

COPY --from=publish /app/publish .

ENTRYPOINT ["./wait-for-it.sh", "electronictestsystem-db:5432", "--timeout=30", "--strict", "--", "dotnet", "ElectronicTestSystem.WebApi.dll"]