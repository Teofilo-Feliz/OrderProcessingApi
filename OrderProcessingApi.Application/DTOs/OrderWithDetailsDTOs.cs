using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.DTOs
{
    public record OrderWithDetailsDTOs(
        int Id,
        DateTime FechaPedido,
        string Estado,
        decimal Total,
        List<OrderDetailDTOs> Productos
        );
}
