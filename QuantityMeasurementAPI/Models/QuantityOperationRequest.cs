using QuantityMeasurementModelLayer.DTO;
 
namespace QuantityMeasurementAPI.Models;
 
public class QuantityOperationRequest
{
    public QuantityDTO Q1 { get; set; }
    public QuantityDTO Q2 { get; set; }
}
