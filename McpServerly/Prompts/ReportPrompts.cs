using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace McpServerly.Prompts;

[McpServerPromptType]
public class ReportPrompts
{
    private static int _calls = 1;
    private readonly ILogger<ReportPrompts> _logger;

    public ReportPrompts(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<ReportPrompts>();
    }
    [McpServerPrompt, Description("Create clients reports")]
    public ChatMessage GenerateReport([Description("The content html for generate report")] string html)
    {
        ChatMessage message = new(ChatRole.User, "Please, make any thing");
        _calls++;
        _logger.LogInformation($"Generating report for {_calls} calls");
        return message;
    }
}