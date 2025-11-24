using System.ComponentModel.DataAnnotations;

namespace ProjectBakamitai.DTO
{
    public class UpdatePasswordDTO
    {
        public string Email { get; set; }

        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }

}