using System;
using System.Collections.Generic;

namespace CAS.Models;

public partial class Drug
{
    public int DrugId { get; set; }

    public string DrugTitle { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? Expiry { get; set; }

    public string? Dosage { get; set; }

    public string DrugStatus { get; set; } = null!;

    public virtual ICollection<PhysicianPrescrip> PhysicianPrescrips { get; set; } = new List<PhysicianPrescrip>();

    public virtual ICollection<PurchaseProductLine> PurchaseProductLines { get; set; } = new List<PurchaseProductLine>();
}
