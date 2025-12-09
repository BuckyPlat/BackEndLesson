namespace Projectbakamitai.DTO
{
    public class InventoryDTO
    {
        public int InvenId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string? Description { get; set; }
        public string? ProductImage { get; set; }
        public string? ItemType { get; set; }

        public int Quantity { get; set; }

        public int? PurchasePriceGold { get; set; }
        public int? PurchasePriceGem { get; set; }
        public DateTime? PurchasedAt { get; set; }
    }

}
