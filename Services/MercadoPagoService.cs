using BuscoProfe.Api.DTOs.Payments;
using BuscoProfe.Api.Interfaces;
using BuscoProfe.Api.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BuscoProfe.Api.Services;

public class MercadoPagoService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;
    private readonly MercadoPagoOptions _options;

    public MercadoPagoService(HttpClient httpClient, IOptions<MercadoPagoOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<MercadoPagoSubscriptionResponseDto> CreateSubscriptionAsync(CreateMercadoPagoSubscriptionDto dto)
    {
        if (!_options.Enabled)
        {
            return new MercadoPagoSubscriptionResponseDto
            {
                IsStandBy = true,
                Success = false,
                Message = "Mercado Pago está configurado en standby. La integración está preparada pero deshabilitada por configuración."
            };
        }

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        var payload = new
        {
            reason = dto.Reason,
            auto_recurring = new
            {
                frequency = dto.Frequency,
                frequency_type = dto.FrequencyType,
                transaction_amount = dto.Amount,
                currency_id = dto.CurrencyId
            },
            back_url = dto.BackUrl,
            payer_email = dto.PayerEmail,
            external_reference = $"membership-{dto.MembershipId}-institution-{dto.InstitutionUserId}",
            status = "pending"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/preapproval", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new MercadoPagoSubscriptionResponseDto
            {
                IsStandBy = false,
                Success = false,
                Message = $"Mercado Pago respondió con error: {response.StatusCode}. Detalle: {responseBody}"
            };
        }

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        string? initPoint = root.TryGetProperty("init_point", out var initPointProp)
            ? initPointProp.GetString()
            : null;

        string? id = root.TryGetProperty("id", out var idProp)
            ? idProp.GetString()
            : null;

        return new MercadoPagoSubscriptionResponseDto
        {
            IsStandBy = false,
            Success = true,
            Message = "Suscripción generada correctamente en Mercado Pago.",
            InitPoint = initPoint,
            MercadoPagoSubscriptionId = id
        };
    }
}