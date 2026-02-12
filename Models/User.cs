using System;
using System.Collections.Generic;

namespace CAS.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? RoleReferenceId { get; set; }

    public string Status { get; set; } = null!;
}
