FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElectronicTestSystem.ConsoleApi/ElectronicTestSystem.ConsoleApi.csproj", "ElectronicTestSystem.ConsoleApi/"]
RUN dotnet restore "ElectronicTestSystem.ConsoleApi/ElectronicTestSystem.ConsoleApi.csproj"
COPY . .
WORKDIR "/src/ElectronicTestSystem.ConsoleApi"
RUN dotnet build "ElectronicTestSystem.ConsoleApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ElectronicTestSystem.ConsoleApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElectronicTestSystem.ConsoleApi.dll"]
