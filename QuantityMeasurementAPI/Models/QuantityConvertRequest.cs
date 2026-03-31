using QuantityMeasurementModelLayer.DTO;
 
namespace QuantityMeasurementAPI.Models;
 
public class QuantityConvertRequest
{
    public QuantityDTO Input      { get; set; }
    public string      TargetUnit { get; set; }
}
