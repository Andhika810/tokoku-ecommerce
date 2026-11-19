namespace ECommerce.Core.Settings;

public class JwtSettings
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "ECommerce";
    public string Audience { get; set; } = "ECommerce";
}