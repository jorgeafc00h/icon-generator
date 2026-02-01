#!/usr/bin/env dotnet-script
#r "nuget: Azure.AI.OpenAI, 2.1.0-beta.2"
#r "nuget: Azure.Identity, 1.12.0"

using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using System;
using System.ClientModel;
using System.Threading.Tasks;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
var deployment = Environment.GetEnvironmentVariable("GPT4O_MINI_DEPLOYMENT_NAME");

Console.WriteLine("Testing Azure OpenAI GPT-4o-mini Connection...\n");
Console.WriteLine($"Endpoint: {endpoint}");
Console.WriteLine($"Deployment: {deployment}\n");

try
{
    var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    var chatClient = client.GetChatClient(deployment);

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage("You are an expert icon designer. Enhance prompts for DALL-E 3."),
        new UserChatMessage("Enhance this for an icon: fitness tracker")
    };

    Console.WriteLine("Sending test request...\n");

    var response = await chatClient.CompleteChatAsync(messages);

    Console.WriteLine("✅ SUCCESS! GPT-4o-mini is working!");
    Console.WriteLine($"\nResponse:\n{response.Value.Content[0].Text}\n");
    Console.WriteLine($"Model: {response.Value.Model}");
    Console.WriteLine($"Tokens: {response.Value.Usage.TotalTokenCount}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR: {ex.Message}");
}
