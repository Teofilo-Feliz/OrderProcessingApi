
using Microsoft.AspNetCore.Mvc;
using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Interfaces;

namespace OrderProcessingApi.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrder _orderService;

    public OrdersController(IOrder orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetOrdersAsync(
            cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.GetOrderByIdAsync(
            id,
            cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new
            {
                mensaje = "Pedido no encontrado.",
                errores = result.Errors
            });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDTOs order,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateOrderAsync(
            order,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                mensaje = "No fue posible procesar el pedido.",
                errores = result.Errors
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            mensaje = "Pedido creado correctamente.",
            pedidoId = result.Value
        });
    }
}
