using Microsoft.Extensions.Logging;
using Moq;
using Services.Implementations;

namespace Tests.TestServices;

public class MailServiceTests
{
    private readonly ILoggerFactory _loggerFactory;

    public MailServiceTests()
    {
        _loggerFactory = new LoggerFactory();
    }
    
}