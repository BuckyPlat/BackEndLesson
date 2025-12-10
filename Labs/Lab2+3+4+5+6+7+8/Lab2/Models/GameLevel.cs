using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Lab2.Models
{
    public class GameLevel
    {
        [Key]
        public int LevelId { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
    }
}
