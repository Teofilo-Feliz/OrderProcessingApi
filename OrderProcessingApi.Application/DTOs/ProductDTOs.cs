using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.DTOs
{
    public record ProductDTOs(
        int Id,
        string Nombre,
        decimal Precio,
        int Stock
        );
}
