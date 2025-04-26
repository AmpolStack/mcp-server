using Services.Implementations;

namespace Tests.TestServices;

public class PdfGeneratorServiceTest
{
    [Fact]
    public async Task WhenAnyInputIsNull()
    {
        //Arrange
        var service = new PdfGeneratorService();
        
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
        var service = new PdfGeneratorService();
        
        //Act
        var resp = await service.ConvertHtmlStringToPdf("<h1>Template title</h1><p>lorem ipsum</p>", "none");
        
        //Assert
        Assert.False(resp.Success);
        Assert.Null(resp.CompletePath);
        Assert.Null(resp.ExtensionPath);
    }
    
    
}