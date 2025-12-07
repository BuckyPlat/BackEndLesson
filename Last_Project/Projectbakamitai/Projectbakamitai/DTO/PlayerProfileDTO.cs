namespace Projectbakamitai.DTO
{
    public class PlayerProfileDTO
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string DisplayName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public int Levels { get; set; }
        public int Exps { get; set; }
        public int Gold { get; set; }
        public int Gem { get; set; }
    }
}
