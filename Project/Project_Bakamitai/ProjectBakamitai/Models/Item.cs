using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Item
{
    public byte ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ItemType { get; set; } = null!;

    public int Price { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
