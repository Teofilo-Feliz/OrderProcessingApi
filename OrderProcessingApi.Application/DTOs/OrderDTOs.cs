using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.DTOs
{
    public record OrderDTOs(
        int Id,
        DateTime FechaPedido,
        decimal Total,
        string Estado
        );
}
