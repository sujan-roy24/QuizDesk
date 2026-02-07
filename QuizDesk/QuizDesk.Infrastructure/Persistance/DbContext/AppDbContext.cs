using Microsoft.EntityFrameworkCore;
using QuizDesk.Domain.Entities;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<UserOAuthAccount> UserOAuthAccounts => Set<UserOAuthAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Console.WriteLine("AppDbContext Connection");
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<User>(entity =>
        {
            entity.OwnsOne(u => u.Email, e =>
            {
                e.Property(x => x.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(256)
                    .IsRequired();

                e.HasIndex(x => x.Value).IsUnique();
            });

            entity.OwnsOne(u => u.PasswordHash, p =>
            {
                p.Property(x => x.Hash)
                    .HasColumnName("PasswordHash");
            });

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(u => u.AuthMethod)
                .HasConversion<string>()
                .HasMaxLength(50);

        });

       
        modelBuilder.Entity<UserOAuthAccount>(entity =>
        {
            entity.OwnsOne(o => o.Email, e =>
            {
                e.Property(x => x.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(256)
                    .IsRequired();
            });

            entity.HasIndex(o => new { o.Provider, o.ProviderUserId })
                .IsUnique();
        });
    }
}