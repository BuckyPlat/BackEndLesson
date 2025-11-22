using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Inventory
{
    public byte CharacterId { get; set; }

    public byte ItemId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? AcquireDate { get; set; }

    public virtual Character Character { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}
