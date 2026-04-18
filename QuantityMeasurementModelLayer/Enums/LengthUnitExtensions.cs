 using QuantityMeasurementModelLayer.Enums;
 namespace QuantityMeasurementModelLayer.Enums;
 public static class LengthUnitExtensions
    {
        public static double GetConversionFactor(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.METER: return 1.0;
                case LengthUnit.KILOMETER: return 1000.0;
                case LengthUnit.FEET: return 1.0;
                case LengthUnit.INCHES: return 1.0 / 12.0;
                case LengthUnit.YARDS: return 3.0;
                case LengthUnit.CENTIMETERS: return 1.0 / 100.0;
                default: throw new ArgumentException();
            }
        }

        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            return value * unit.GetConversionFactor();
        }

        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
        {
            return baseValue / unit.GetConversionFactor();
        }

        public static string GetUnitName(this LengthUnit unit)
        {
            return unit.ToString();
        }
    }
