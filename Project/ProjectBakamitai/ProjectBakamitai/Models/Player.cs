using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Player
{
    public byte Playerid { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] Passwordhash { get; set; } = null!;

    public DateTime? Createdate { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
