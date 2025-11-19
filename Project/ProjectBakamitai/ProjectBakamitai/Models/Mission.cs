using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Mission
{
    public byte Missionid { get; set; }

    public string Missionname { get; set; } = null!;

    public byte? Difficulty { get; set; }

    public int? Rewardgold { get; set; }

    public byte? Rewarditemid { get; set; }

    public virtual ICollection<Playermission> Playermissions { get; set; } = new List<Playermission>();

    public virtual Item? Rewarditem { get; set; }
}
