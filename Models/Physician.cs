using System;
using System.Collections.Generic;

namespace CAS.Models;

public partial class Physician
{
    public int PhysicianId { get; set; }

    public string PhysicianName { get; set; } = null!;

    public string? Specialization { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Summary { get; set; }

    public string PhysicianStatus { get; set; } = null!;

    public virtual ICollection<DrugRequest> DrugRequests { get; set; } = new List<DrugRequest>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
