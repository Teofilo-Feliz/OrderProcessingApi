using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.DTOs
{
    public record CreateOrderDTOs(
    List<CreateOrderDetailDTOs> Productos
    );
}
