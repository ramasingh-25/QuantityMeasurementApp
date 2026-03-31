
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Context;
using QuantityMeasurementRepositoryLayer.Interfaces;
 
namespace QuantityMeasurementRepositoryLayer.Repositories;
 
public class QuantityMeasurementEFRepository : IQuantityMeasurementRepositorySql
{
    private readonly QuantityMeasurementDbContext _context;
 
    // DbContext injected by DI
    public QuantityMeasurementEFRepository(QuantityMeasurementDbContext context)
    {
        _context = context;
    }
 
    // Save 
    // INSERT INTO QuantityMeasurements (Operation, Operand1, Operand2, Result)
    // VALUES (@Operation, @Operand1, @Operand2, @Result)
    public void Save(QuantityMeasurementEntity entity)
    {
        try
        {
            _context.Measurements.Add(entity);  
            _context.SaveChanges();              
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database save error: {ex.Message}");
            throw;
        }
    }
 
    // GetAll
    // SELECT Operation, Operand1, Operand2, Result FROM QuantityMeasurements
    public List<QuantityMeasurementEntity> GetAll()
    {
        return _context.Measurements.ToList();  
    }
}
 
