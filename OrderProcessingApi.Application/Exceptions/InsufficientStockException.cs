using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public IReadOnlyList<string> RejectedProducts { get; }

        public InsufficientStockException(IEnumerable<string> rejectedProducts)
            : base("No fue posible completar el pedido por problemas de stock.")
        {
            RejectedProducts = rejectedProducts.ToList();

            if (RejectedProducts.Count == 0)
            {
                throw new ArgumentException(
                    "Debes especificar al menos un producto rechazado.",
                    nameof(rejectedProducts));
            }
        }
    }
}
