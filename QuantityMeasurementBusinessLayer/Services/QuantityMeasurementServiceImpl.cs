using QuantityMeasurementBusinessLayer.Interfaces;
using QuantityMeasurementModelLayer.DTO;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementModelLayer.Enums;
using QuantityMeasurementRepositoryLayer.Interfaces;

namespace QuantityMeasurementBusinessLayer.Services;

public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
{
    private readonly IQuantityMeasurementRepositorySql repository;

    public QuantityMeasurementServiceImpl(IQuantityMeasurementRepositorySql repo)
    {
        repository = repo;
    }

    private double ConvertToBase(double value, string unit)
    {
        if (Enum.TryParse(unit, out LengthUnit length))   return value * length.GetConversionFactor();
        if (Enum.TryParse(unit, out WeightUnit weight))   return value * weight.GetConversionFactor();
        if (Enum.TryParse(unit, out VolumeUnit volume))   return value * volume.ToBaseUnit();
        if (Enum.TryParse(unit, out TemperatureUnit temp))
        {
            if (temp == TemperatureUnit.CELSIUS)    return value;
            if (temp == TemperatureUnit.FAHRENHEIT) return (value - 32) * 5 / 9;
        }
        throw new ArgumentException($"Unsupported unit: {unit}");
    }

    private string GetMeasurementType(string unit)
    {
        if (Enum.TryParse<LengthUnit>(unit, out _))      return "LENGTH";
        if (Enum.TryParse<WeightUnit>(unit, out _))      return "WEIGHT";
        if (Enum.TryParse<VolumeUnit>(unit, out _))      return "VOLUME";
        if (Enum.TryParse<TemperatureUnit>(unit, out _)) return "TEMPERATURE";
        return "UNKNOWN";
    }

    public bool Compare(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        bool result = v1 == v2;
        repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "COMPARE", result ? 1 : 0, GetMeasurementType(q1.Unit)));
        return result;
    }

    public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        double value = v1 + v2;
        repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "ADD", value, GetMeasurementType(q1.Unit)));
        return new QuantityDTO(value, q1.Unit);
    }

    public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        double value = v1 - v2;
        repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "SUBTRACT", value, GetMeasurementType(q1.Unit)));
        return new QuantityDTO(value, q1.Unit);
    }

    public double Divide(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        if (v2 == 0) throw new ArithmeticException("Division by zero");
        double result = v1 / v2;
        repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "DIVIDE", result, GetMeasurementType(q1.Unit)));
        return result;
    }

    public QuantityDTO Convert(QuantityDTO input, string targetUnit)
    {
        double baseValue = ConvertToBase(input.Value, input.Unit);
        double converted;

        if (Enum.TryParse(targetUnit, out LengthUnit length))
            converted = baseValue / length.GetConversionFactor();
        else if (Enum.TryParse(targetUnit, out WeightUnit weight))
            converted = baseValue / weight.GetConversionFactor();
        else if (Enum.TryParse(targetUnit, out VolumeUnit volume))
            converted = baseValue / volume.ToBaseUnit();
        else if (Enum.TryParse(targetUnit, out TemperatureUnit temp))
            converted = temp == TemperatureUnit.FAHRENHEIT ? baseValue * 9 / 5 + 32 : baseValue;
        else
            throw new ArgumentException($"Unsupported target unit: {targetUnit}");

        repository.Save(new QuantityMeasurementEntity(input.Value, input.Unit, 0, targetUnit, "CONVERT", converted, GetMeasurementType(input.Unit)));
        return new QuantityDTO(converted, targetUnit);
    }

    public List<QuantityMeasurementEntity> GetAll() => repository.GetAll();
    public List<QuantityMeasurementEntity> GetCacheHistory() => repository.GetAll();
    public List<QuantityMeasurementEntity> GetRedisHistory() => repository.GetAll();
    public List<QuantityMeasurementEntity> GetSqlHistory() => repository.GetAll();
    public List<QuantityMeasurementEntity> GetEFHistory() => repository.GetAll();
}
