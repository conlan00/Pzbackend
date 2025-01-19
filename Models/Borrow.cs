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

    public int? BorrowShelterId { get; set; }

    public int? ReturnShelterId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Shelter? BorrowShelter { get; set; }

    public virtual Shelter? ReturnShelter { get; set; }

    public virtual User User { get; set; } = null!;
}
