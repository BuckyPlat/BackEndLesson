using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Player
{
    public byte PlayerId { get; set; }

    public string PlayerName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
