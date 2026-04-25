namespace BuscoProfe.Api.Options;

public class MercadoPagoOptions
{
    public bool Enabled { get; set; } = false;
    public string AccessToken { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.mercadopago.com";
}