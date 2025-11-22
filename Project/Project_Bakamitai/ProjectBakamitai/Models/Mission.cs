using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Mission
{
    public byte MissionId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Expreward { get; set; }

    public int? GoldReward { get; set; }

    public virtual ICollection<CharacterMission> CharacterMissions { get; set; } = new List<CharacterMission>();
}
