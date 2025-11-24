namespace ProjectBakamitai.Models
{
    public partial class ShopItem
    {
        public byte ShopId { get; set; }
        public byte ItemId { get; set; }

        public virtual Shop Shop { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;
    }
}
