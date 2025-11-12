using System;
using System.Collections.Generic;

namespace DataFirst.Models;

public partial class Shop
{
    public byte ShopId { get; set; }

    public string ItemName { get; set; } = null!;

    public int? Price { get; set; }

    public int? Quality { get; set; }
}
