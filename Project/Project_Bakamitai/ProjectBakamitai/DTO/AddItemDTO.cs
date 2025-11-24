namespace ProjectBakamitai.DTO
{
    public class AddItemDTO
    {
        public string ItemName { get; set; } = null!;
        public string ItemType { get; set; } = null!;
        public int Price { get; set; }
        public int? ExpValue { get; set; }
    }
}
