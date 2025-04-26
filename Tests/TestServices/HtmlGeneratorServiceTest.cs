using System.Diagnostics.CodeAnalysis;
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
        var service = new HtmlGeneratorService();
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GenerateFromMarkdownString(null!));
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
        var service = new HtmlGeneratorService();
        
        //Act
        var resp = service.GenerateFromMarkdownString(md);
        
        //Assert
        Assert.NotEmpty(resp);
        Assert.NotNull(resp);
        Assert.Equal(resp, html);
    }
}