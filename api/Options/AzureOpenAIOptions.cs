namespace IconGenerator.Functions.Options;

public class AzureOpenAIOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DallE3Deployment { get; set; } = "dalle3-icon-generator";
    public string Gpt4oMiniDeployment { get; set; } = "gpt-4o-mini-prompts";
}
