namespace Services.Configurations;

public class MongoSettings
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? Database { get; set; }

    public string GetConnectionString()
    {
        return $"mongodb://{Username}:{Password}@{Host}:{Port}";
    }
    
}