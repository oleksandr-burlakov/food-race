namespace API.IoC;

public class UnleashSettings
{
    public const string SectionName = "Unleash";
    public string? AppName { get; set; }
    public string? ApiUrl { get; set; }
    public string? AuthorizationToken { get; set; }
}