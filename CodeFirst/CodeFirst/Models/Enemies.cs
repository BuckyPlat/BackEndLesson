using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models
{
    [Table("Enemies")]
    public class Enemies
    {
        [Key]
        public int EnemyId { get; set; }
        [Required]
        public string EnemyName { get; set; }
        [MaxLength(20)]
        public int EnemyAttack { get; set; }

        public int Speed { get; set; }

        public string EnemyType { get; set; }
    }
}
