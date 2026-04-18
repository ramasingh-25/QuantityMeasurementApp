FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["QuantityMeasurementWebAPI.csproj", "./"]
COPY ["../QuantityMeasurementBusinessLayer/QuantityMeasurementBusinessLayer.csproj", "../QuantityMeasurementBusinessLayer/"]
COPY ["../QuantityMeasurementRepositoryLayer/QuantityMeasurementRepositoryLayer.csproj", "../QuantityMeasurementRepositoryLayer/"]
COPY ["../QuantityMeasurementModelLayer/QuantityMeasurementModelLayer.csproj", "../QuantityMeasurementModelLayer/"]
RUN dotnet restore "./QuantityMeasurementWebAPI.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/."
RUN dotnet build "QuantityMeasurementWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
