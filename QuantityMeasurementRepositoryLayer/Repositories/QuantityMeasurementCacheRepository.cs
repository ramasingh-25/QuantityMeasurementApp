using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interfaces;

namespace QuantityMeasurementRepositoryLayer.Repositories;

public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
{
    private readonly List<QuantityMeasurementEntity> cache = new();
    private int nextId = 1;

    public void Save(QuantityMeasurementEntity entity)
    {
        entity.Id = nextId++;
        cache.Add(entity);
    }

    public List<QuantityMeasurementEntity> GetAll() => new(cache);

    public List<QuantityMeasurementEntity> GetByOperation(string operation) =>
        cache.FindAll(e => e.Operation.Equals(operation, StringComparison.OrdinalIgnoreCase));

    public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType) =>
        cache.FindAll(e => e.MeasurementType.Equals(measurementType, StringComparison.OrdinalIgnoreCase));

    public int GetTotalCount() => cache.Count;

    public void DeleteAll() => cache.Clear();

    public bool OperationExists(double firstValue, string firstUnit, double secondValue, string secondUnit, string operation) =>
        cache.Exists(e =>
            e.FirstValue == firstValue &&
            e.FirstUnit == firstUnit &&
            e.SecondValue == secondValue &&
            e.SecondUnit == secondUnit &&
            e.Operation == operation);

    public QuantityMeasurementEntity GetLastSavedOperation() =>
        cache.Count > 0 ? cache[cache.Count - 1] : null!;

    public bool TestConnection() => true;

    public void ResetIdentity() { nextId = 1; }
}
