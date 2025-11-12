using System;
using System.Collections.Generic;

namespace DataFirst.Models;

public partial class Player
{
    public byte Id { get; set; }

    public string Username { get; set; } = null!;

    public int? Levi { get; set; }

    public int? Gold { get; set; }
}
