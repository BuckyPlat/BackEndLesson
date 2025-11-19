using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Playermission
{
    public byte Playermissionid { get; set; }

    public byte Characterid { get; set; }

    public byte Missionid { get; set; }

    public byte? Status { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Character Character { get; set; } = null!;

    public virtual Mission Mission { get; set; } = null!;
}
