using System;
using System.Collections.Generic;

namespace Projectbakamitai.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ProductImage { get; set; }

    public int? PriceGold { get; set; }

    public int? PriceGem { get; set; }

    public string? ItemType { get; set; }

    public bool? IsShow { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<ShopProduct> ShopProducts { get; set; } = new List<ShopProduct>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
