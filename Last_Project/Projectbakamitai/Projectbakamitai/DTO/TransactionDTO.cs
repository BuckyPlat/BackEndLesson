namespace Projectbakamitai.DTO
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string? ProductImage { get; set; }
        public string TransactionType { get; set; }
        public string CurrencyType { get; set; }
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }
    }

}
