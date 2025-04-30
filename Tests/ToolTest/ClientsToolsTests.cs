using McpServerly.Tools;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Definitions;
using Services.Models;

namespace Tests.ToolTest;

public class ClientsToolsTests
{
    private readonly Mock<IGenericRepository> _repository;
    private readonly Mock<ILogger<ClientsTools>> _logger;

    public ClientsToolsTests()
    {
        _repository = new Mock<IGenericRepository>();
        _logger = new Mock<ILogger<ClientsTools>>();
        
    }

    [Fact]
    public async Task GetClientsTest()
    {
        //Arrange
        var clients = new List<Client>()
        {
            new Client()
            {
                Id = "1",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Test",
                Address = new Address()
                {
                    City =  "Test",
                    State = "Test",
                    Street = "Test",
                    ZipCode = "12345"
                },
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Phone = "12345",
            }, 
            new Client()
            {
                Id = "2",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Test",
                Address = new Address()
                {
                    City =  "Test",
                    State = "Test",
                    Street = "Test",
                    ZipCode = "12345"
                },
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Phone = "12345",
            },
        };
        
        _repository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clients);
        
        
        var service = new ClientsTools(_repository.Object, _logger.Object);
        
        //Act
        var resp = await service.GetClients();
        
        //Arrange
        Assert.NotNull(resp);
        Assert.NotEmpty(resp);
        Assert.Equal(clients, resp);
        _repository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}