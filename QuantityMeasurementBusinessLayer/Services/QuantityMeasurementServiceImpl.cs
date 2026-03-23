using QuantityMeasurementBusinessLayer.Interfaces;
using QuantityMeasurementModelLayer.DTO;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementModelLayer.Enums;
using QuantityMeasurementModelLayer.Extensions;
using QuantityMeasurementRepositoryLayer.Interfaces;

namespace QuantityMeasurementBusinessLayer.Services;

public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
{
    public readonly IQuantityMeasurementRepository Repository;

    public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repo)
    {
        Repository = repo;
    }

    private double ConvertToBase(double value, string unit)
    {
        if (Enum.TryParse(unit, out LengthUnit length))
            return value * length.GetConversionFactor();

        if (Enum.TryParse(unit, out WeightUnit weight))
            return value * weight.GetConversionFactor();

        if (Enum.TryParse(unit, out VolumeUnit volume))
            return value * volume.ToBaseUnit();

        if (Enum.TryParse(unit, out TemperatureUnit temp))
        {
            return temp switch
            {
                TemperatureUnit.CELSIUS => value,
                TemperatureUnit.FAHRENHEIT => (value - 32) * 5 / 9,
                _ => throw new ArgumentException("Unsupported temperature unit")
            };
        }

        throw new ArgumentException($"Unsupported unit: {unit}");
    }

    public bool Compare(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        bool result = v1 == v2;
        Repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "COMPARE", result ? 1 : 0, GetMeasurementType(q1.Unit)));
        return result;
    }

    public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        double value = v1 + v2;
        Repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "ADD", value, GetMeasurementType(q1.Unit)));
        return new QuantityDTO(value, q1.Unit);
    }

    public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        double value = v1 - v2;
        Repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "SUBTRACT", value, GetMeasurementType(q1.Unit)));
        return new QuantityDTO(value, q1.Unit);
    }

    public double Divide(QuantityDTO q1, QuantityDTO q2)
    {
        double v1 = ConvertToBase(q1.Value, q1.Unit);
        double v2 = ConvertToBase(q2.Value, q2.Unit);
        if (v2 == 0) throw new ArithmeticException("Division by zero");
        double result = v1 / v2;
        Repository.Save(new QuantityMeasurementEntity(q1.Value, q1.Unit, q2.Value, q2.Unit, "DIVIDE", result, GetMeasurementType(q1.Unit)));
        return result;
    }

    public QuantityDTO Convert(QuantityDTO input, string targetUnit)
    {
        double baseValue = ConvertToBase(input.Value, input.Unit);

        if (Enum.TryParse(targetUnit, out LengthUnit length))
            return new QuantityDTO(baseValue / length.GetConversionFactor(), targetUnit);

        if (Enum.TryParse(targetUnit, out WeightUnit weight))
            return new QuantityDTO(baseValue / weight.GetConversionFactor(), targetUnit);

        if (Enum.TryParse(targetUnit, out VolumeUnit volume))
            return new QuantityDTO(baseValue / volume.ToBaseUnit(), targetUnit);

        if (Enum.TryParse(targetUnit, out TemperatureUnit temp))
        {
            double converted = temp switch
            {
                TemperatureUnit.CELSIUS => baseValue,
                TemperatureUnit.FAHRENHEIT => baseValue * 9 / 5 + 32,
                _ => throw new ArgumentException("Unsupported temperature unit")
            };
            return new QuantityDTO(converted, targetUnit);
        }

        throw new ArgumentException($"Unsupported target unit: {targetUnit}");
    }

    private string GetMeasurementType(string unit)
    {
        if (Enum.TryParse<LengthUnit>(unit, out _)) return "LENGTH";
        if (Enum.TryParse<WeightUnit>(unit, out _)) return "WEIGHT";
        if (Enum.TryParse<VolumeUnit>(unit, out _)) return "VOLUME";
        if (Enum.TryParse<TemperatureUnit>(unit, out _)) return "TEMPERATURE";
        return "UNKNOWN";
    }
}
