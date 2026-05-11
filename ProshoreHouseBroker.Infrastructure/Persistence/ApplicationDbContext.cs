using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PropertyListing> PropertyListings => Set<PropertyListing>();

    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();

    public DbSet<CommissionRule> CommissionRules => Set<CommissionRule>();

    public DbSet<PropertyBooking> PropertyBookings => Set<PropertyBooking>();

    public DbSet<ErrorInfo> ErrorInfos => Set<ErrorInfo>();

    public DbSet<AppActivity> AppActivities => Set<AppActivity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PropertyListing>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(250).IsRequired();
            entity.Property(x => x.PropertyType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Features).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CommissionAmount).HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.Broker)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.BrokerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Images)
                .WithOne(x => x.PropertyListing)
                .HasForeignKey(x => x.PropertyListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PropertyImage>()
            .Property(x => x.ImageUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Entity<PropertyBooking>(entity =>
        {
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.AdminCommissionAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.BrokerNetCommissionAmount).HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.PropertyListing)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.PropertyListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.HouseSeeker)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.HouseSeekerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CommissionRule>(entity =>
        {
            entity.Property(x => x.MinAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Percentage).HasColumnType("decimal(5,2)");
            entity.Property(x => x.AdminSharePercentage).HasColumnType("decimal(5,2)");
        });

        builder.Entity<ErrorInfo>(entity =>
        {
            entity.Property(x => x.Message).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.ExceptionType).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Path).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Method).HasMaxLength(20).IsRequired();
            entity.Property(x => x.UserId).HasMaxLength(100);
            entity.Property(x => x.DeviceId).HasMaxLength(200);
            entity.Property(x => x.IpAddress).HasMaxLength(100);
        });

        builder.Entity<AppActivity>(entity =>
        {
            entity.Property(x => x.Path).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Method).HasMaxLength(20).IsRequired();
            entity.Property(x => x.EndpointName).HasMaxLength(1000);
            entity.Property(x => x.UserId).HasMaxLength(100);
            entity.Property(x => x.DeviceId).HasMaxLength(200);
            entity.Property(x => x.IpAddress).HasMaxLength(100);
        });
    }
}
