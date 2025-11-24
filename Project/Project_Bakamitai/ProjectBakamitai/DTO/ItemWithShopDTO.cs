namespace ProjectBakamitai.DTO
{
    public class ItemWithShopDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int Price { get; set; }
        public List<string> ShopNames { get; set; } = new();
    }
}
