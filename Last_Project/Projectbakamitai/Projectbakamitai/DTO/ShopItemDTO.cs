namespace Projectbakamitai.DTO
{
    public class ShopItemDTO
    {
        public int ShopItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Description { get; set; }
        public string? ProductImage { get; set; }
        public int? PriceGold { get; set; }
        public int? PriceGem { get; set; }
        public string? ItemType { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
