using System.ComponentModel.DataAnnotations;

namespace ProjectBakamitai.DTO
{
    public class AddItemDTO
    {
        [Required]
        [MaxLength(50)]
        public string ItemName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemType { get; set; }

        [Range(1, int.MaxValue)]
        public int Price { get; set; }
    }

}
