using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab2.Views.ViewModel
{
    public class RegisterVM
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public string LinkAvatar { get; set; }
        public required int RegionId { get; set; }
        public IEnumerable<SelectListItem> Regions { get; set; }
    }
}
