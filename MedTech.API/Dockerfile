FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MedTech.API/MedTech.API.csproj", "MedTech.API/"]
COPY ["MedTech.Core/MedTech.Core.csproj", "MedTech.Core/"]
COPY ["MedTech.Infrastructure/MedTech.Infrastructure.csproj", "MedTech.Infrastructure/"]

RUN dotnet restore "MedTech.API/MedTech.API.csproj"

COPY MedTech.API/ MedTech.API/
COPY MedTech.Core/ MedTech.Core/
COPY MedTech.Infrastructure/ MedTech.Infrastructure/

WORKDIR "/src/MedTech.API"
RUN dotnet publish "MedTech.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "MedTech.API.dll"]
