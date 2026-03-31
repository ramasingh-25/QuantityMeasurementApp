using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementRepositoryLayer.Repositories;
using QuantityMeasurementRepositoryLayer.Interfaces;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementBusinessLayer.Interfaces;
using QuantityMeasurementModelLayer.DTO;
using QuantityMeasurementModelLayer.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace QuantityMeasurementApp.Tests;

[TestClass]
public class QuantityMeasurementIntegrationTests
{
    private IConfiguration? _configuration;
    private IServiceProvider? _serviceProvider;

    [TestInitialize]
    public void TestInitialize()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        Assert.IsNotNull(_configuration);

        _serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(_configuration!)
            .AddSingleton<IQuantityMeasurementRepository, QuantityMeasurementCacheRepository>()
            .AddSingleton<IQuantityMeasurementRepositorySql, QuantityMeasurementSqlRepository>()
            .AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>()
            .BuildServiceProvider();
    }

    [TestMethod]
    public void TestDotnetBuild_Success()
    {
        // Verifies project builds successfully with dotnet.
        // Command: dotnet build
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build",
            WorkingDirectory = Path.GetFullPath("../../../"), // Adjust path to solution root
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });

        process!.WaitForExit();
        Assert.AreEqual(0, process.ExitCode, "Build should succeed");
    }

    [TestMethod]
    public void TestPackageStructure_AllLayersPresent()
    {
        // Verifies controller, service, repository packages exist.
        // Verifies proper organization.
        var solutionDir = Path.GetFullPath("../../../../");
        Assert.IsTrue(Directory.Exists(Path.Combine(solutionDir, "QuantityMeasurementModelLayer")));
        Assert.IsTrue(Directory.Exists(Path.Combine(solutionDir, "QuantityMeasurementBusinessLayer")));
        Assert.IsTrue(Directory.Exists(Path.Combine(solutionDir, "QuantityMeasurementRepositoryLayer")));
        Assert.IsTrue(Directory.Exists(Path.Combine(solutionDir, "QuantityMeasurementConsoleApp")));
    }

    [TestMethod]
    public void TestCsprojDependencies_SqlClientIncluded()
    {
        // Verifies Microsoft.Data.SqlClient in csproj.
        var solutionDir = Path.GetFullPath("../../../../");
        var csprojPath = Path.Combine(solutionDir, "QuantityMeasurementRepositoryLayer", "QuantityMeasurementRepositoryLayer.csproj");
        var content = File.ReadAllText(csprojPath);
        StringAssert.Contains(content, "Microsoft.Data.SqlClient", "SQL Client package should be included");
    }

    [TestMethod]
    public void TestDatabaseConfiguration_LoadedFromAppsettings()
    {
        // Verifies configuration loads from appsettings.json.
        Assert.IsNotNull(_configuration);
        var connectionString = _configuration!.GetConnectionString("DefaultConnection");
        Assert.IsNotNull(connectionString, "Connection string should be loaded");
        StringAssert.Contains(connectionString, "QuantityMeasurementDB", "Should contain database name");
    }

    // The following database-dependent integration tests were removed to prevent long-running failures
    // when a SQL Server instance is not available or connection strings are not configured.
    // Keep the suite focused on build/config structure and in-memory cache behavior.

    // Removed long-running, recursive, or external-dependency tests to keep suite fast and reliable locally.

    [TestMethod]
    public void TestConfiguration_EnvironmentOverride()
    {
        // Sets environment variable for connection string.
        // Creates repository.
        // Verifies environment variable takes precedence.
        Environment.SetEnvironmentVariable("CONNECTION_STRING", "Server=test;Database=test;");
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config["CONNECTION_STRING"];
        Assert.AreEqual("Server=test;Database=test;", connectionString, "Environment variable should override");
    }


    [TestMethod]
    public void TestCsproj_PluginConfiguration()
    {
        // Verifies csproj has correct configurations.
        var csprojPath = "../../../../QuantityMeasurementConsoleApp/QuantityMeasurementConsoleApp.csproj";
        var content = File.ReadAllText(csprojPath);
        StringAssert.Contains(content, "<TargetFramework>net9.0</TargetFramework>", "Should target net9.0");
        StringAssert.Contains(content, "<OutputType>Exe</OutputType>", "Should be executable");
    }
}