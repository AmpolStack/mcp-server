using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Implementations;

namespace Tests.TestServices;

public class HtmlGeneratorServiceTest
{
    private readonly Mock<ILoggerFactory> _loggerFactory;

    public HtmlGeneratorServiceTest()
    {
        var factory = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger<HtmlGeneratorService>>();
        factory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        _loggerFactory = factory;
    }
    
    [Fact]
    public void WhenTheInputIsNull()
    {
        //Arrange
        var logger = _loggerFactory;
        var service = new HtmlGeneratorService(logger.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GenerateFromMarkdownString(null!));
        logger.Verify(f => f.CreateLogger(It.Is<string>(s => s.Contains(nameof(HtmlGeneratorService)))), Times.Once);
    }
    
    [Theory]
    [InlineData("hello", "<p>hello</p>\n")]
    [InlineData("*i*", "<p><em>i</em></p>\n")]
    [InlineData("**b**", "<p><strong>b</strong></p>\n")]
    [InlineData("`x`", "<p><code>x</code></p>\n")]
    [InlineData("[l](u)", "<p><a href=\"u\">l</a></p>\n")]
    public void ProveInputs(string md, string html)
    {
        //Arrange
        var logger = _loggerFactory;
        var service = new HtmlGeneratorService(logger.Object);
        
        //Act
        var resp = service.GenerateFromMarkdownString(md);
        
        //Assert
        Assert.NotEmpty(resp);
        Assert.NotNull(resp);
        Assert.Equal(resp, html);
    }
}