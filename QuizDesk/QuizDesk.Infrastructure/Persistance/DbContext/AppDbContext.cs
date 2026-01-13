using Microsoft.EntityFrameworkCore;
using QuizDesk.Domain.Entities;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<UserOAuthAccount> UserOAuthAccounts { get; set; }
    public DbSet<OAuthProvider> OAuthProviders { get; set; }

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
                    .HasColumnName("PasswordHash")
                    .HasMaxLength(500);
            });

            entity.Property(u => u.FullName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(u => u.AuthMethod)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

        });

       
        modelBuilder.Entity<UserOAuthAccount>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Provider)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(o => o.ProviderUserId)
                .HasMaxLength(200)
                .IsRequired();

            entity.OwnsOne(o => o.Email, e =>
            {
                e.Property(x => x.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(256)
                    .IsRequired();
            });

            entity.HasIndex(o => new { o.Provider, o.ProviderUserId })
                .IsUnique();

            entity.HasIndex(o => new { o.Provider, o.UserId })
                .IsUnique();

            entity.HasOne(o => o.User)
                .WithMany(u => u.OAuthAccounts)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OAuthProvider>(entity =>
        {
            entity.Property(o => o.Scopes)
                .HasColumnType("text[]");
        });
    }
}