using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Helpers;


namespace OrderProcessingApi.Application.Interfaces
{
    public interface IProduct
    {
        Task<List<ProductDTOs>> GetProductsAsync(
         CancellationToken cancellationToken = default);

        Task<Result<ProductDTOs>> GetProductByIdAsync(
         int id,
         CancellationToken cancellationToken = default);

        Task<Result<int>> CreateProductAsync(
         CreateProductDTOs product,
         CancellationToken cancellationToken = default);
    }
}
