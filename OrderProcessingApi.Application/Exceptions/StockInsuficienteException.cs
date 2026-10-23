using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.Exceptions
{
    public class StockInsuficienteException : Exception
    {
        public IReadOnlyList<string> ProductosRechazados { get; }

        public StockInsuficienteException(IEnumerable<string> productosRechazados)
            : base("No fue posible completar el pedido por problemas de stock.")
        {
            ProductosRechazados = productosRechazados.ToList();

            if (ProductosRechazados.Count == 0)
            {
                throw new ArgumentException(
                    "Debes especificar al menos un producto rechazado.",
                    nameof(productosRechazados));
            }
        }
    }
}
