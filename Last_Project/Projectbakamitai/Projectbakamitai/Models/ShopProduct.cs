using System;
using System.Collections.Generic;

namespace Projectbakamitai.Models;

public partial class ShopProduct
{
    public int ShopItemId { get; set; }

    public int ItemId { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual Item Item { get; set; } = null!;
}
