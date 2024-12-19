using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class OperationHistory
{
    public int Id { get; set; }

    public string OperationDescription { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime DateTime { get; set; }

    public virtual User User { get; set; } = null!;
}
