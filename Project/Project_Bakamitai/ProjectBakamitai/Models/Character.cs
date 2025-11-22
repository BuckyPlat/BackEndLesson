using System;
using System.Collections.Generic;

namespace ProjectBakamitai.Models;

public partial class Character
{
    public byte CharacterId { get; set; }

    public byte PlayerId { get; set; }

    public string CharacterName { get; set; } = null!;

    public byte GamemodeId { get; set; }

    public int? Heatlh { get; set; }

    public int? Hunger { get; set; }

    public int? Experience { get; set; }

    public int? Gold { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<CharacterMission> CharacterMissions { get; set; } = new List<CharacterMission>();

    public virtual GameMode Gamemode { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Player Player { get; set; } = null!;
}
