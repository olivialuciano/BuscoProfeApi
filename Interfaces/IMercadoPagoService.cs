using BuscoProfe.Api.DTOs.Payments;

namespace BuscoProfe.Api.Interfaces;

public interface IMercadoPagoService
{
    Task<MercadoPagoSubscriptionResponseDto> CreateSubscriptionAsync(CreateMercadoPagoSubscriptionDto dto);
}