using GameOfLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infra.EntityFramework
{
    public class GameOfLifeDbContext : DbContext
    {
        public GameOfLifeDbContext(DbContextOptions<GameOfLifeDbContext> options) 
            : base(options)
        {
            
        }

        public DbSet<Cell> Cells { get; set; }
        public DbSet<Board> Boards { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cell>(entity =>
            {
                entity.HasKey(e => new {e.BoardId, e.PositionX, e.PositionY});
            });

            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(e => e.BoardId);
                entity.HasMany(e => e.LivingCells)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
