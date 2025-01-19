using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Shelter
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public float Long { get; set; }

    public float Lat { get; set; }

    public virtual ICollection<BookArrival> BookArrivals { get; set; } = new List<BookArrival>();

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Borrow> BorrowBorrowShelters { get; set; } = new List<Borrow>();

    public virtual ICollection<Borrow> BorrowReturnShelters { get; set; } = new List<Borrow>();
}
