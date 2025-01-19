using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Cover { get; set; } = null!;

    public int CategoryId { get; set; }

    public int? ShelterId { get; set; }

    public virtual ICollection<BookArrival> BookArrivals { get; set; } = new List<BookArrival>();

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<LikedBook> LikedBooks { get; set; } = new List<LikedBook>();

    public virtual Shelter? Shelter { get; set; }
}
