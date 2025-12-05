using System;
using System.Collections.Generic;

namespace Projectbakamitai.Models;

public partial class Inventory
{
    public int InvenId { get; set; }

    public int UserId { get; set; }

    public int ItemId { get; set; }

    public int? Quantity { get; set; }

    public int? PurchasePriceGold { get; set; }

    public int? PurchasePriceGem { get; set; }

    public DateTime? PurchasedAt { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
