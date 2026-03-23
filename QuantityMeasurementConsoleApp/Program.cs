using QuantityMeasurementConsoleApp.Interfaces;
using QuantityMeasurementConsoleApp.Services;

class Program
{
    static void Main()
    {
        IMenu menu = new Menu();
        menu.Start();
    }
}
