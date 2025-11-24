using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Shop
{
    public byte ShopId { get; set; }

    public string ShopName { get; set; } = null!;

    public virtual ICollection<ShopItem> ShopItems { get; set; } = new List<ShopItem>();
}
