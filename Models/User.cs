using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Points { get; set; }

    public string Token { get; set; } = null!;

    public virtual ICollection<BookArrival> BookArrivals { get; set; } = new List<BookArrival>();

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();

    public virtual ICollection<LikedBook> LikedBooks { get; set; } = new List<LikedBook>();

    public virtual ICollection<OperationHistory> OperationHistories { get; set; } = new List<OperationHistory>();
}
