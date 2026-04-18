FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire solution
COPY ../ . .
# Restore dependencies
RUN dotnet restore "./QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj"

# Build the application
RUN dotnet build "./QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
