using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.Data
{
    public class EnemiesContext : DbContext
    {
        public EnemiesContext(DbContextOptions<EnemiesContext> options) : base(options)
        {
        }
        public DbSet<Enemies> enemies { get; set; }
    }
}
