using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interfaces;
 
namespace QuantityMeasurementRepositoryLayer.Repositories;
public class QuantityMeasurementRedisRepository : IQuantityMeasurementRepository
{
    private readonly IDistributedCache _cache;
    private const string MasterKey = "all_measurement_keys";
 
    // IDistributedCache is injected by DI 
    public QuantityMeasurementRedisRepository(IDistributedCache cache)
    {
        _cache = cache;
    }
 
    public void Save(QuantityMeasurementEntity entity)
    {
       
        string entityKey = "measurement_" + Guid.NewGuid().ToString();
 
        
        string json = JsonSerializer.Serialize(entity);
 
        
        _cache.SetString(entityKey, json);
 
        
        string existingJson = _cache.GetString(MasterKey) ?? "[]";
        List<string> allKeys = JsonSerializer.Deserialize<List<string>>(existingJson)!;
        allKeys.Add(entityKey);
        _cache.SetString(MasterKey, JsonSerializer.Serialize(allKeys));
    }
 
    public List<QuantityMeasurementEntity> GetAll()
    {
       
        string existingJson = _cache.GetString(MasterKey) ?? "[]";
        List<string> allKeys = JsonSerializer.Deserialize<List<string>>(existingJson)!;
 
        
        var entities = new List<QuantityMeasurementEntity>();
        foreach (string key in allKeys)
        {
            string? json = _cache.GetString(key);
            if (json != null)
            {
                var entity = JsonSerializer.Deserialize<QuantityMeasurementEntity>(json);
                if (entity != null)
                    entities.Add(entity);
            }
        }
 
        return entities;
    }
}