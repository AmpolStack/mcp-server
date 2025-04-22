using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Implementations;

namespace Tests.TestServices;

public class HtmlGeneratorServiceTest
{

    private static Mock<ILoggerFactory> ComposeFactoryMock()
    {
        var factory = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger<HtmlGeneratorService>>();
        factory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        return factory;
    }
    
    [Fact]
    public void WhenTheInputIsNull()
    {
        //Arrange
        var factory = ComposeFactoryMock();
        var service = new HtmlGeneratorService(factory.Object);
        
        //Act
        var resp = service.GenerateFromMarkdownString(null);
        //Assert
        Assert.Empty(resp);
        factory.Verify(f => f.CreateLogger(It.Is<string>(s => s.Contains(nameof(HtmlGeneratorService)))), Times.Once);
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
        var factory = ComposeFactoryMock();
        var service = new HtmlGeneratorService(factory.Object);
        
        //Act
        var resp = service.GenerateFromMarkdownString(md);
        
        //Assert
        Assert.NotEmpty(resp);
        Assert.NotNull(resp);
        Assert.Equal(resp, html);
    }
}