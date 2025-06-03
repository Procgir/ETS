FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElectronicTestSystem.ConsoleAppV3/ElectronicTestSystem.ConsoleAppV3.csproj", "ElectronicTestSystem.ConsoleAppV3/"]
RUN dotnet restore "ElectronicTestSystem.ConsoleAppV3/ElectronicTestSystem.ConsoleAppV3.csproj"
COPY . .
WORKDIR "/src/ElectronicTestSystem.ConsoleAppV3"
RUN dotnet build "ElectronicTestSystem.ConsoleAppV3.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ElectronicTestSystem.ConsoleAppV3.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElectronicTestSystem.ConsoleAppV3.dll"]
