namespace Api.Settings;

public class DbSettings
{
    public required DbInfo Authentication { get; set; }
    public required DbInfo Logs { get; set; }
}

public class DbInfo
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}