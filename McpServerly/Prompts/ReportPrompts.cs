using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Services.Definitions;

namespace McpServerly.Prompts;

[McpServerPromptType]
public class ReportPrompts
{
    private static int calls = 1;
    private readonly ILogger<ReportPrompts> _logger;

    public ReportPrompts(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<ReportPrompts>();
    }
    [McpServerPrompt, Description("Create clients reports")]
    public ChatMessage GenerateReport([Description("The content html for generate report")] string html)
    {
        ChatMessage message = new(ChatRole.User, "Please, make any thing");
        calls++;
        _logger.LogInformation($"Generating report for {calls} calls");
        return message;
    }
}