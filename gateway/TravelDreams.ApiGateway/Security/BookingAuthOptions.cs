namespace TravelDreams.ApiGateway.Security;

public sealed class BookingAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TravelDreams.BookingIntegration";
    public string Audience { get; set; } = "TravelDreams.Booking";
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
