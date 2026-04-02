using Spectre.Console;

namespace AP.UseMethodForAI;

public class DotnetMethodHelper
{
    public static string CalculateTax(string customer, int forMonths)
    {
        AnsiConsole.WriteLine($"Calculating tax for customer {customer} for past {forMonths} months.");
        var multiplier = 2;
        var result = customer switch
        {
            "Method" => forMonths * multiplier,
            "Tax" => (double)forMonths / multiplier,
            _ => forMonths
        };
        var tax = $"Tax for customer {customer}  for past {forMonths} months is {result}";
        return tax;
    }
}