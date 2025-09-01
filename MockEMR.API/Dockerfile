FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MockEMR.API/MockEMR.API.csproj", "MockEMR.API/"]

RUN dotnet restore "MockEMR.API/MockEMR.API.csproj"

COPY MockEMR.API/ MockEMR.API/

WORKDIR "/src/MockEMR.API"
RUN dotnet publish "MockEMR.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "MockEMR.API.dll"]
