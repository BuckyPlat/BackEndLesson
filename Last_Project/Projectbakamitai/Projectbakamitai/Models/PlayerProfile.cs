using System;
using System.Collections.Generic;

namespace Projectbakamitai.Models;

public partial class PlayerProfile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public string DisplayName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public int? Levels { get; set; }

    public int? Exps { get; set; }

    public int? Gold { get; set; }

    public int? Gem { get; set; }

    public virtual User User { get; set; } = null!;
}
