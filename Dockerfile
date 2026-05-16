# Use the official .NET 8 SDK to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["backend/CarStock.API/CarStock.API.csproj", "backend/CarStock.API/"]
RUN dotnet restore "backend/CarStock.API/CarStock.API.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/backend/CarStock.API"
RUN dotnet build "CarStock.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "CarStock.API.csproj" -c Release -o /app/publish

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarStock.API.dll"]