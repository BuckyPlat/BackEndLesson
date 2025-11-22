using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class GameMode
{
    public byte GamemodeId { get; set; }

    public string ModeName { get; set; } = null!;

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
