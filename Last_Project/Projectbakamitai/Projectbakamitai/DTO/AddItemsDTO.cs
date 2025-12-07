using System.ComponentModel.DataAnnotations;

namespace Projectbakamitai.DTO
{
    public class AddItemsDTO
    {
        [Required]
        public string ItemName { get; set; }
        public string? Description { get; set; }
        public int? PriceGold { get; set; }
        public int? PriceGem { get; set; }
        public string ItemType { get; set; }
        public bool? IsShow { get; set; }
        public IFormFile? ProductImage { get; set; }
    }
}
