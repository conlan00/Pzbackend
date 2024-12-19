using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Category
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
