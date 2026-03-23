using System;
using QuantityMeasurementBusinessLayer.Interfaces;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementModelLayer.DTO;
using QuantityMeasurementRepositoryLayer.Interfaces;
using QuantityMeasurementRepositoryLayer.Repositories;
using QuantityMeasurementConsoleApp.Interfaces;

namespace QuantityMeasurementConsoleApp.Services;

public class Menu : IMenu
{
    private readonly IQuantityMeasurementService service;
    private readonly IQuantityMeasurementRepository repository;

    public Menu()
    {
        repository = new QuantityMeasurementCacheRepository();
        service = new QuantityMeasurementServiceImpl(repository);
    }

    public void Start()
    {
        while (true)
        {
            try
            {
                Console.WriteLine("\n===== Quantity Measurement System =====");
                Console.WriteLine("1 Length");
                Console.WriteLine("2 Volume");
                Console.WriteLine("3 Weight");
                Console.WriteLine("4 Temperature");
                Console.WriteLine("5 Show Records");
                Console.WriteLine("6 Exit");

                Console.Write("Enter choice: ");
                int type = Convert.ToInt32(Console.ReadLine());

                if (type == 6) return;

                if (type == 5)
                {
                    ShowAllData();
                    continue;
                }

                OperationMenu(type);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    private void OperationMenu(int type)
    {
        Console.WriteLine("\nSelect Operation");
        Console.WriteLine("1 Add");
        Console.WriteLine("2 Subtract");
        Console.WriteLine("3 Divide");
        Console.WriteLine("4 Compare");
        Console.WriteLine("5 Convert");

        Console.Write("Enter Operation: ");
        int operation = Convert.ToInt32(Console.ReadLine());

        QuantityDTO firstValue = GetQuantity(type);

        if (operation == 5)
        {
            ConvertUnit(firstValue, type);
            return;
        }

        QuantityDTO secondValue = GetQuantity(type);

        switch (operation)
        {
            case 1:
                PrintResult(service.Add(firstValue, secondValue));
                break;
            case 2:
                PrintResult(service.Subtract(firstValue, secondValue));
                break;
            case 3:
                Console.WriteLine("Result = " + service.Divide(firstValue, secondValue));
                break;
            case 4:
                Console.WriteLine("Are Equal = " + service.Compare(firstValue, secondValue));
                break;
            default:
                Console.WriteLine("Invalid Operation");
                break;
        }
    }

    private QuantityDTO GetQuantity(int type)
    {
        Console.Write("\nEnter Value: ");
        double value = Convert.ToDouble(Console.ReadLine());
        string unit = SelectUnit(type);
        return new QuantityDTO(value, unit);
    }

    private string SelectUnit(int type)
    {
        Console.WriteLine("Select Unit");

        if (type == 1)
        {
            Console.WriteLine("1 FEET\n2 INCHES\n3 YARDS\n4 CENTIMETERS");
            return Convert.ToInt32(Console.ReadLine()) switch
            {
                1 => "FEET", 2 => "INCHES", 3 => "YARDS", 4 => "CENTIMETERS", _ => "FEET"
            };
        }
        else if (type == 2)
        {
            Console.WriteLine("1 LITRE\n2 MILLILITRE\n3 GALLON");
            return Convert.ToInt32(Console.ReadLine()) switch
            {
                1 => "LITRE", 2 => "MILLILITRE", 3 => "GALLON", _ => "LITRE"
            };
        }
        else if (type == 3)
        {
            Console.WriteLine("1 KILOGRAM\n2 GRAM\n3 POUND");
            return Convert.ToInt32(Console.ReadLine()) switch
            {
                1 => "KILOGRAM", 2 => "GRAM", 3 => "POUND", _ => "KILOGRAM"
            };
        }
        else
        {
            Console.WriteLine("1 CELSIUS\n2 FAHRENHEIT");
            return Convert.ToInt32(Console.ReadLine()) switch
            {
                1 => "CELSIUS", 2 => "FAHRENHEIT", _ => "CELSIUS"
            };
        }
    }

    private void ConvertUnit(QuantityDTO firstValue, int type)
    {
        Console.WriteLine("\nSelect Target Unit");
        string targetUnit = SelectUnit(type);
        PrintResult(service.Convert(firstValue, targetUnit));
    }

    private void PrintResult(QuantityDTO result)
    {
        Console.WriteLine($"\nResult = {result.Value} {result.Unit}");
    }

    private void ShowAllData()
    {
        var list = repository.GetAll();

        Console.WriteLine("\n===== STORED RECORDS =====");
        foreach (var item in list)
        {
            Console.WriteLine(
                $"Id: {item.Id} | " +
                $"FirstValue: {item.FirstValue} {item.FirstUnit} | " +
                $"SecondValue: {item.SecondValue} {item.SecondUnit} | " +
                $"Operation: {item.Operation} | " +
                $"Result: {item.Result} | " +
                $"Type: {item.MeasurementType}"
            );
        }
        Console.WriteLine("==========================\n");
    }
}
