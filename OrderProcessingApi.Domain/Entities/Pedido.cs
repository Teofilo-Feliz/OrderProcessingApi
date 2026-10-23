using System;
using System.Collections.Generic;

namespace OrderProcessingApi.Domain.Entities;

public partial class Pedido
{
    public int Id { get; set; }

    public DateTime FechaPedido { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
