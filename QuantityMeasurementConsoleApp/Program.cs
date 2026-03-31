using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementBusinessLayer.Interfaces;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementRepositoryLayer.Interfaces;
using QuantityMeasurementRepositoryLayer.Repositories;

class Program
{
    static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var provider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<IQuantityMeasurementRepository, QuantityMeasurementCacheRepository>()
            .AddSingleton<IQuantityMeasurementRepositorySql, QuantityMeasurementSqlRepository>()
            .AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>()
            .BuildServiceProvider();

        var service = provider.GetService<IQuantityMeasurementService>();

        Menu menu = new Menu(service);
        menu.Start();
    }
}