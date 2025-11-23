namespace ProjectBakamitai.DTO
{
    public class CharacterResponseDTO
    {
        public byte CharacterId { get; set; }
        public string CharacterName { get; set; } = null!;
        public string PlayerName { get; set; } = null!;
        public string GamemodeName { get; set; } = null!;
    }
}
