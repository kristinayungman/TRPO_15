using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pr15_Shop.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public double Rating { get; set; }

    public DateOnly CreatedAt { get; set; }

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;

    public int BrandId { get; set; }

    [NotMapped]
    public string DisplayTags { get; set; } = string.Empty;
}
