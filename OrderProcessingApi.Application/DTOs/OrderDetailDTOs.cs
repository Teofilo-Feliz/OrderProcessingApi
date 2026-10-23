using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.DTOs
{
    public record OrderDetailDTOs(
        int ProductoId,
        string Producto,
        int Cantidad,
        decimal PrecioUnitario,
        decimal Subtotal
        );
}
