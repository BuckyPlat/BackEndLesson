using Lab2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Lab2.Models.GameLevel> GameLevels { get; set; }
        public DbSet<Lab2.Models.Question> Questions { get; set; }
        public DbSet<Lab2.Models.LevelResult> LevelResults { get; set; }
        public DbSet<Lab2.Models.Region> Regions { get; set; }
        public DbSet<Lab2.Models.ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modeBuilder)
        {
            base.OnModelCreating(modeBuilder);
            modeBuilder.Entity<GameLevel>().HasData(
                new GameLevel { LevelId = 1, title = "Cấp độ 1" },
                new GameLevel { LevelId = 2, title = "Cấp độ 2" },
                new GameLevel { LevelId = 3, title = "Cấp độ 3" }
            );
            modeBuilder.Entity<Region>().HasData(
                new Region { RegionId = 1, Name = "Đồng bằng sông hồng" },
                new Region { RegionId = 2, Name = "Đồng bằng sông cửu long" }
            );
            modeBuilder.Entity<Question>().HasData(
                new Question
                {
                    QuestionId = 1,
                    ContentQuestion = "Câu hỏi 1",
                    Answer = "Đáp án 1",
                    Option1 = "Đáp án 1",
                    Option2 = "Đáp án 2",
                    Option3 = "Đáp án 3",
                    Option4 = "Đáp án 4",
                    LevelId = 1
                },
                new Question
                {
                    QuestionId = 2,
                    ContentQuestion = "Câu hỏi 2",
                    Answer = "Đáp án 2",
                    Option1 = "Đáp án 1",
                    Option2 = "Đáp án 2",
                    Option3 = "Đáp án 3",
                    Option4 = "Đáp án 4",
                    LevelId = 2
                }
                );
        }
    }
}
