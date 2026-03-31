using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interfaces;

namespace QuantityMeasurementRepositoryLayer.Repositories;

public class QuantityMeasurementSqlRepository : IQuantityMeasurementRepositorySql
{
    private readonly string _connectionString;

    public QuantityMeasurementSqlRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    }

    public void Save(QuantityMeasurementEntity entity)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var command = new SqlCommand(
            "INSERT INTO [QuantityMeasurements] ([Operation], [FirstValue], [FirstUnit], [SecondValue], [SecondUnit], [Result], [MeasurementType]) " +
            "VALUES (@op, @fv, @fu, @sv, @su, @r, @mt)", connection);
        command.Parameters.AddWithValue("@op", entity.Operation);
        command.Parameters.AddWithValue("@fv", entity.FirstValue);
        command.Parameters.AddWithValue("@fu", entity.FirstUnit);
        command.Parameters.AddWithValue("@sv", entity.SecondValue);
        command.Parameters.AddWithValue("@su", entity.SecondUnit);
        command.Parameters.AddWithValue("@r", entity.Result);
        command.Parameters.AddWithValue("@mt", entity.MeasurementType);
        command.ExecuteNonQuery();
    }

    public List<QuantityMeasurementEntity> GetAll()
    {
        var entities = new List<QuantityMeasurementEntity>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var command = new SqlCommand(
            "SELECT [Id], [Operation], [FirstValue], [FirstUnit], [SecondValue], [SecondUnit], [Result], [MeasurementType] FROM [QuantityMeasurements]", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            entities.Add(new QuantityMeasurementEntity(
                reader.GetDouble(2),
                reader.GetString(3),
                reader.GetDouble(4),
                reader.GetString(5),
                reader.GetString(1),
                reader.GetDouble(6),
                reader.GetString(7)
            ) { Id = reader.GetInt32(0) });
        }
        return entities;
    }
}
