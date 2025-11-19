using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Transaction
{
    public byte Transactionid { get; set; }

    public byte Playerid { get; set; }

    public byte? Itemid { get; set; }

    public int Amount { get; set; }

    public string? Description { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Player Player { get; set; } = null!;
}
