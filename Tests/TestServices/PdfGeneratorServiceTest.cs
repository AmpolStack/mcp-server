using Services.Implementations;

namespace Tests.TestServices;

public class PdfGeneratorServiceTest
{
    [Fact]
    public async Task WhenAnyInputIsNull()
    {
        //Arrange
        var service = new PdfGeneratorService();
        
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async ()=> await service.ConvertHtmlStringToPdf(null!, null!));
        
    }

    [Fact]
    public async Task WhenOutputPathNotExists()
    {
        //Arrange
        var service = new PdfGeneratorService();
        
        //Act
        var resp = await service.ConvertHtmlStringToPdf("<h1>Template title</h1><p>lorem ipsum</p>", "none");
    }
    
    
}