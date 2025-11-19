using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Item
{
    public byte Itemid { get; set; }

    public string Itemname { get; set; } = null!;

    public string? Itemtype { get; set; }

    public string? Description { get; set; }

    public int Price { get; set; }

    public virtual ICollection<Inventoryitem> Inventoryitems { get; set; } = new List<Inventoryitem>();

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
