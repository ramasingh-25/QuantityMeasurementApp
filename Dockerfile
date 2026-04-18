FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire solution
COPY . .
# Restore the solution
RUN dotnet restore "QuantityMeasurementApp.sln"

# Build the web API project
RUN dotnet build "QuantityMeasurementApp.sln" -c Release -p:QuantityMeasurementWebAPI -o /app/build

FROM build AS publish
RUN dotnet publish "QuantityMeasurementApp.sln" -c Release -p:QuantityMeasurementWebAPI -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
