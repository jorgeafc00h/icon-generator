namespace IconGenerator.Functions.Options;

public class DatabaseOptions
{
    public string Type { get; set; } = "cosmosdb"; // cosmosdb or sql

    // Cosmos DB
    public string? CosmosEndpoint { get; set; }
    public string? CosmosKey { get; set; }
    public string CosmosDatabase { get; set; } = "IconGeneratorDB";

    // SQL Database
    public string? SqlConnectionString { get; set; }
}
