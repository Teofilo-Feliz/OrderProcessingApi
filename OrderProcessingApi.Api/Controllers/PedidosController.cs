
using Microsoft.AspNetCore.Mvc;
using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Interfaces;

namespace OrderProcessingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IOrder _orderService;

    public PedidosController(IOrder orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearPedido(
        [FromBody] CreateOrderDTOs pedido,
        CancellationToken cancellationToken)
    {
        var resultado = await _orderService.CrearPedidoAsync(
            pedido,
            cancellationToken);

        if (resultado.IsFailure)
        {
            return BadRequest(new
            {
                mensaje = "No fue posible procesar el pedido.",
                errores = resultado.Errors
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            mensaje = "Pedido creado correctamente.",
            pedidoId = resultado.Value
        });
    }
}
