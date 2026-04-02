using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AP.TaxServer.Tools;

[McpServerToolType]
public class McpTools(ILogger<McpTools> logger)
{
    [McpServerTool(Name = "get_tax_for_customer",
        Title = "Gets tax for customer for specific period of months")]
    [Description("Gets tax for customer for specific period of month")]
    public string CalculateTax(string customer, int forMonths)
    {
        logger.LogInformation($"Calculating tax for customer {customer} for past {forMonths} months.");
        var multiplier = 2;
        var result = customer switch
        {
            "Method" => forMonths * multiplier,
            "Tax" => (double)forMonths / multiplier,
            _ => forMonths
        };
        var tax = $"Tax for customer {customer}  for past {forMonths} months is {result}";
        logger.LogInformation(tax);
        return tax;
    }
}