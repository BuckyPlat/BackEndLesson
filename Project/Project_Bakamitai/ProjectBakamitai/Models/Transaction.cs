namespace ProjectBakamitai.Models
{
    public partial class Transaction
    {
        public int TransactionId { get; set; }
        public byte CharacterId { get; set; }
        public byte ShopId { get; set; }
        public byte ItemId { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime TransactionDate { get; set; }

        public Character Character { get; set; }
        public Shop Shop { get; set; }
        public Item Item { get; set; }
    }
}
