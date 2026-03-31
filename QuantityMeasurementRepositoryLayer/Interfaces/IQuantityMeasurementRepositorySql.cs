using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Interfaces;

public interface IQuantityMeasurementRepositorySql
{
    void Save(QuantityMeasurementEntity entity);
    List<QuantityMeasurementEntity> GetAll();
}