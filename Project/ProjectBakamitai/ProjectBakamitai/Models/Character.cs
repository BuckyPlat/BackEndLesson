using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Character
{
    public byte Characterid { get; set; }

    public byte Playerid { get; set; }

    public string Charactername { get; set; } = null!;

    public int? Level { get; set; }

    public int? Experience { get; set; }

    public int? Health { get; set; }

    public int? Hunger { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Player Player { get; set; } = null!;

    public virtual ICollection<Playermission> Playermissions { get; set; } = new List<Playermission>();
}
