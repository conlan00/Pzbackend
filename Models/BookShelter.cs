using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class BookShelter
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public int ShelterId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Shelter Shelter { get; set; } = null!;
}
