# Deploy to Render (Manual Setup)

This guide will help you deploy your Quantity Measurement API to Render. Since Render doesn't support .NET applications in blueprint files, we'll set up the services manually.

## Prerequisites

1. A Render account (https://render.com)
2. Your code pushed to a GitHub repository
3. A credit card on file (for free tier usage)

## Deployment Steps

### 1. Push to GitHub

First, make sure your code is pushed to GitHub:

```bash
git add .
git commit -m "Configure for Render deployment"
git push origin main
```

### 2. Create Render Account

1. Go to https://render.com
2. Sign up/login with your GitHub account
3. Add a payment method (required for free tier)

### 3. Create PostgreSQL Database

1. Go to your Render dashboard
2. Click "New +" and select "PostgreSQL"
3. **Name**: quantity-measurement-db
4. **Database Name**: quantitymeasurementdb
5. **User**: postgres (or create new user)
6. **Region**: Choose closest to your users
7. **Plan**: Free
8. Click "Create Database"

### 4. Create Web API Service

1. Go to your Render dashboard
2. Click "New +" and select "Web Service"
3. Connect your GitHub repository
4. Configure the service:

#### Web Service Configuration

- **Name**: quantity-measurement-api
- **Environment**: Docker
- **Region**: Same as your database
- **Branch**: main (or your target branch)
- **Root Directory**: `QuantityMeasurementWebAPI` (if needed)
- **Instance Type**: Free

#### Docker Configuration

Since Render doesn't have native .NET support, we'll use Docker:

1. **Create a Dockerfile** (I'll help you create this)
2. **Runtime**: Docker
3. **Build Command**: `docker build -t quantity-measurement-api .`
4. **Start Command**: `docker run -p 10000:80 quantity-measurement-api`

### 5. Create Dockerfile

Create this file in your `QuantityMeasurementWebAPI` directory:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["QuantityMeasurementWebAPI.csproj", "./"]
RUN dotnet restore "./QuantityMeasurementWebAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "QuantityMeasurementWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
```

### 6. Environment Variables

Add these to your web service:

1. Go to your web service dashboard
2. Click "Environment" tab
3. Add these environment variables:

- `ASPNETCORE_ENVIRONMENT` = `Production`
- `JWT__Key` = (generate a secure key, e.g., using a password generator)
- `ConnectionStrings__DefaultConnection` = (get this from your PostgreSQL database dashboard)

The connection string format should be:
```
Host=your-db-host.railway.app;Database=quantitymeasurementdb;Username=postgres;Password=your-password
```

### 7. Alternative: Direct .NET Deployment

If Docker doesn't work, try this approach:

#### Web Service Configuration

1. **Service Type**: Web Service
2. **Name**: quantity-measurement-api
3. **Environment**: Docker (still required)
4. **Build Context**: Set to `QuantityMeasurementWebAPI`
5. **Dockerfile Path**: `Dockerfile`

### 8. Deployment Process

1. Render will automatically build and deploy your application
2. The PostgreSQL database is already running
3. Your API will be available at: `https://quantity-measurement-api.onrender.com`

### 9. Accessing Your API

- **Swagger UI**: `https://quantity-measurement-api.onrender.com/swagger`
- **API Base URL**: `https://quantity-measurement-api.onrender.com`

### 10. Testing the Deployment

1. Visit the Swagger UI to test endpoints
2. Check the logs in Render dashboard if there are issues
3. Verify database connection by checking if tables were created

## Troubleshooting

### Common Issues

1. **Build Failures**: Check the build logs in Render dashboard
2. **Database Connection**: Ensure PostgreSQL is running and connection string is correct
3. **CORS Issues**: The API is configured to allow all origins in production
4. **JWT Issues**: Verify JWT key is properly set as environment variable
5. **Docker Issues**: Make sure Dockerfile is in the correct directory

### Logs

Access your application logs in the Render dashboard under your service > Logs.

### Database Access

You can access your PostgreSQL database directly using the connection string provided in the Render dashboard.

## Post-Deployment

1. Update any frontend applications to use the new API URL
2. Monitor your usage in the Render dashboard
3. Set up alerts if needed (available in paid plans)

## Features Configured

- **Automatic HTTPS**: Render provides SSL certificates
- **Automatic Deployments**: New commits trigger rebuilds
- **Database**: PostgreSQL with automatic backups
- **Environment Variables**: Secure configuration management
- **CORS**: Configured for cross-origin requests
- **Swagger**: Available for API testing

## Quick Summary

1. Create PostgreSQL database in Render
2. Create Dockerfile in QuantityMeasurementWebAPI folder
3. Create Web Service with Docker environment
4. Set environment variables
5. Deploy and test

Your application is now ready for deployment to Render!
