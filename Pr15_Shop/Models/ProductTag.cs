using System;
using System.Collections.Generic;

namespace Pr15_Shop.Models;

public partial class ProductTag
{
    public int ProductId { get; set; }

    public int TagId { get; set; }
    //public virtual Tag Tag { get; set; }
}
