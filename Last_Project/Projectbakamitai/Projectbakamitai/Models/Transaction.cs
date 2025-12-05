using System;
using System.Collections.Generic;

namespace Projectbakamitai.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public int ItemId { get; set; }

    public string? TransactionType { get; set; }

    public string? CurrencyType { get; set; }

    public int Amount { get; set; }

    public int? Quantity { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
