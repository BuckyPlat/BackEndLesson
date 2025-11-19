using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Inventoryitem
{
    public byte Inventoryitemid { get; set; }

    public byte Inventoryid { get; set; }

    public byte Itemid { get; set; }

    public int? Quantity { get; set; }

    public virtual Inventory Inventory { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}
