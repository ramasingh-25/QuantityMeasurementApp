using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuantityMeasurementModelLayer.Entities
{
    [Table("QuantityMeasurements")]
    public class QuantityMeasurementEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Operation { get; set; } = string.Empty;

        public double FirstValue { get; set; }

        [MaxLength(50)]
        public string FirstUnit { get; set; } = string.Empty;

        public double SecondValue { get; set; }

        [MaxLength(50)]
        public string SecondUnit { get; set; } = string.Empty;

        public double Result { get; set; }

        [MaxLength(50)]
        public string MeasurementType { get; set; } = string.Empty;

        public QuantityMeasurementEntity() { }

        public QuantityMeasurementEntity(
            double firstValue,
            string firstUnit,
            double secondValue,
            string secondUnit,
            string operation,
            double result,
            string measurementType)
        {
            FirstValue = firstValue;
            FirstUnit = firstUnit ?? string.Empty;
            SecondValue = secondValue;
            SecondUnit = secondUnit ?? string.Empty;
            Operation = operation ?? string.Empty;
            Result = result;
            MeasurementType = measurementType ?? string.Empty;
        }
    }
}
