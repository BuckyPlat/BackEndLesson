using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class CharacterMission
{
    public byte CharacterId { get; set; }

    public byte MissionId { get; set; }

    public bool? IsCompleted { get; set; }

    public DateTime? CompletionDate { get; set; }

    public virtual Character Character { get; set; } = null!;

    public virtual Mission Mission { get; set; } = null!;
}
