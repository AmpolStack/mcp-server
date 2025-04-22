using Microsoft.Extensions.Logging;
using Moq;
using Services.Implementations;

namespace Tests.TestServices;

public class PdfGeneratorServiceTest
{
    private readonly ILoggerFactory _loggerFactory;

    public PdfGeneratorServiceTest()
    {
        var factory = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger<PdfGeneratorService>>();
        factory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        _loggerFactory = factory.Object;
    }
    
    [Fact]
    public async Task WhenAnyInputIsNull()
    {
        //Arrange
        var logger = _loggerFactory!;
        var service = new PdfGeneratorService(logger);
        
        //Act
        var resp = await service.ConvertHtmlStringToPdf(null!, null!);
        
        //Assert
        Assert.False(resp.Success);
        Assert.Null(resp.CompletePath);
        Assert.Null(resp.ExtensionPath);
    }

    [Fact]
    public async Task WhenOutputPathNotExists()
    {
        //Arrange
        var logger = _loggerFactory!;
        var service = new PdfGeneratorService(logger);
        
        //Act
        var resp = await service.ConvertHtmlStringToPdf("<h1>Template title</h1><p>lorem ipsum</p>", "none");
        
        //Assert
        Assert.False(resp.Success);
        Assert.Null(resp.CompletePath);
        Assert.Null(resp.ExtensionPath);
    }
    
    
}