namespace IconGenerator.Functions.Options;

public class StorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "generated-icons";
    public string AssetsContainerName { get; set; } = "app-resources";
}
