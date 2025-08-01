using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MAS_Shared.DBModels;

namespace MAS_Shared.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public new DbSet<ApplicationUserToken> UserTokens { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
                b.Property(e => e.UserId).HasColumnType("nvarchar(450)");
                b.Property(e => e.LoginProvider).HasColumnType("nvarchar(450)");
                b.Property(e => e.Name).HasColumnType("nvarchar(450)");
                b.Property(e => e.Value).HasColumnType("nvarchar(max)");
                b.HasDiscriminator<string>("Discriminator")
                    .HasValue<IdentityUserToken<string>>("IdentityUserToken")
                    .HasValue<ApplicationUserToken>("1");
                b.ToTable("AspNetUserTokens");
            });

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.SocialMedia).HasMaxLength(255).IsRequired(false);
                b.Property(u => u.POPIConsent).IsRequired(false);
                b.Property(u => u.DtTmJoined).HasColumnType("timestamp").IsRequired(false);
                b.Property(u => u.IsVerified).HasDefaultValue(false).IsRequired();

                // Keeping identity configurations intact
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            });

            // Configure Driver entity
            modelBuilder.Entity<Driver>(b =>
            {
                b.HasKey(d => d.UserID);
                b.Property(d => d.UserID).HasMaxLength(450);
                b.Property(d => d.LastOnline).IsRequired(false);
                b.Property(d => d.GPSLat).IsRequired(false);
                b.Property(d => d.GPSLong).IsRequired(false);
                b.Property(d => d.LocationLastUpdated).IsRequired(false);
                b.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(b =>
            {
                b.HasKey(o => o.OrderID);
                b.Property(o => o.UserID).HasMaxLength(450).IsRequired();
                b.Property(o => o.OrderItems).HasMaxLength(255).IsRequired();
                b.Property(o => o.DttmInitiated).IsRequired();
                b.Property(o => o.OrderTotal).HasColumnType("numeric(12, 0)").IsRequired(false);
                b.Property(o => o.DriverID).HasMaxLength(450).IsRequired();
                b.Property(o => o.IsBeingCollected).HasDefaultValue(false).IsRequired();
                b.Property(o => o.IsEnrouteToCust).HasDefaultValue(false).IsRequired();
                b.Property(o => o.IsArrivedAtCust).HasDefaultValue(false).IsRequired();
                b.Property(o => o.IsPaid).HasDefaultValue(false).IsRequired();
                b.Property(o => o.DttmDelivered).IsRequired(false);
                b.Property(o => o.DisputedReason).HasMaxLength(255).IsRequired(false);
                b.Property(o => o.DttmClosed).IsRequired(false);
                b.HasOne<Driver>()
                    .WithMany()
                    .HasForeignKey(o => o.DriverID)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(o => o.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Business entity configuration
            modelBuilder.Entity<Business>()
                .HasKey(b => b.BusinessID);

            modelBuilder.Entity<Business>()
                .HasIndex(b => b.Cellnumber)
                .IsUnique();

            // Catalog entity configuration
            modelBuilder.Entity<Catalog>()
                .HasKey(c => c.CatalogID);

            modelBuilder.Entity<Catalog>()
                .HasOne(c => c.Business)
                .WithMany()
                .HasForeignKey(c => c.BusinessID)
                .OnDelete(DeleteBehavior.Cascade);

            // CtalogItem entity configuration
            modelBuilder.Entity<CatalogItem>()
                .HasKey(ci => new { ci.CatalogID, ci.ItemID });
        }

    }
}
