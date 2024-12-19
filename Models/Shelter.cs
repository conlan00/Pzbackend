using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Shelter
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public float Long { get; set; }

    public float Lat { get; set; }

    public virtual ICollection<BookShelter> BookShelters { get; set; } = new List<BookShelter>();

    public virtual ICollection<Borrow> BorrowShelterId2Navigations { get; set; } = new List<Borrow>();

    public virtual ICollection<Borrow> BorrowShelters { get; set; } = new List<Borrow>();
}
