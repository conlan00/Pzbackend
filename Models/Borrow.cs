using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Borrow
{
    public int Id { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime? ReturnTime { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public int ShelterId2 { get; set; }

    public int? ShelterId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Shelter? Shelter { get; set; }

    public virtual Shelter ShelterId2Navigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
