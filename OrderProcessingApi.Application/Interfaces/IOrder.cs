using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Helpers;


namespace OrderProcessingApi.Application.Interfaces
{
    public interface IOrder
    {
        Task<Result<int>> CreateOrderAsync(
         CreateOrderDTOs order,
         CancellationToken cancellationToken = default);

        Task<List<OrderDTOs>> GetOrdersAsync(
         CancellationToken cancellationToken = default);

        Task<Result<OrderWithDetailsDTOs>> GetOrderByIdAsync(
         int id,
         CancellationToken cancellationToken = default);
    }
}
