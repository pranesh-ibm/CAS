using System;
using System.Collections.Generic;

namespace CAS.Models;

public partial class PurchaseOrderHeader
{
    public int Poid { get; set; }

    public string Pono { get; set; } = null!;

    public DateOnly Podate { get; set; }

    public int SupplierId { get; set; }

    public virtual ICollection<PurchaseProductLine> PurchaseProductLines { get; set; } = new List<PurchaseProductLine>();

    public virtual Supplier Supplier { get; set; } = null!;
}
