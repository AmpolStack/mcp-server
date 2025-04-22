using Microsoft.Extensions.Logging;
using Moq;
using Services.Implementations;

namespace Tests.TestServices;

public class HtmlGeneratorServiceTest
{
    [Fact]
    public void WhenTheInputIsNull()
    {
        //Arrange
        var factory = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger<HtmlGeneratorService>>();
        factory
            .Setup(f => f.CreateLogger(It.Is<string>(s => s.Contains(nameof(HtmlGeneratorService)))))
            .Returns(logger.Object);
        
        var service = new HtmlGeneratorService(factory.Object);
        
        //Act
        var resp = service.GenerateFromMarkdownString(null);
        //Assert
        Assert.Empty(resp);
        factory.Verify(f => f.CreateLogger(It.Is<string>(s => s.Contains(nameof(HtmlGeneratorService)))), Times.Once);
    }
    
}