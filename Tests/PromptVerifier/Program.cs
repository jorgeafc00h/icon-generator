using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Models;

Console.WriteLine("Prompt Verifier - PromptEngineeringService\n");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<PromptEngineeringService>();
var svc = new PromptEngineeringService(logger);

var styles = new[] { "Neon", "3D", "Minimal" };

foreach (var style in styles)
{
    Console.WriteLine($"--- Style: {style} ---\n");
    var system = svc.BuildIconSystemPrompt(style);
    Console.WriteLine("System prompt (sanitized):\n");
    Console.WriteLine(system);
    Console.WriteLine();

    var req = new IconGenerationRequest
    {
        Style = style,
        Keywords = "sample concept for testing",
        Colors = new System.Collections.Generic.List<string> { "#FF00FF", "#00FFFF" },
        Quality = "standard"
    };

    var user = svc.BuildIconUserPrompt(req);
    Console.WriteLine("User prompt (sanitized):\n");
    Console.WriteLine(user);
    Console.WriteLine();
}

Console.WriteLine("Prompt verification complete.");
