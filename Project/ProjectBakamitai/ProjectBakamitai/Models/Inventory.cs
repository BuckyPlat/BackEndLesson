using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Inventory
{
    public byte Inventoryid { get; set; }

    public byte Characterid { get; set; }

    public virtual Character Character { get; set; } = null!;

    public virtual ICollection<Inventoryitem> Inventoryitems { get; set; } = new List<Inventoryitem>();
}
