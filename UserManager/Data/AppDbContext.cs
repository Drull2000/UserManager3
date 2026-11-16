using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UserManager.Models;

namespace UserManager.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

                entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

                entity.HasQueryFilter(u => !u.IsDeleted);
            });
        }
    }
}
