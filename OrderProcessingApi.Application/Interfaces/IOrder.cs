using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Helpers;


namespace OrderProcessingApi.Application.Interfaces
{
    public interface IOrder
    {
        Task<Result<int>> CrearPedidoAsync(
         CreateOrderDTOs pedido,
         CancellationToken cancellationToken = default);
    }
}
