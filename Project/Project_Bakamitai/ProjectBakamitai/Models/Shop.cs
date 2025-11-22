using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Shop
{
    public byte ShopId { get; set; }

    public string ShopName { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
