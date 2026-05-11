using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Domain.Enums;
using ProshoreHouseBroker.Infrastructure.Persistence;

namespace ProshoreHouseBroker.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await dbContext.Database.EnsureCreatedAsync();
        await EnsureSchemaUpgradesAsync(dbContext);

        foreach (var role in new[] { UserRoles.Admin, UserRoles.Broker, UserRoles.HouseSeeker })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        if (!await dbContext.CommissionRules.AnyAsync())
        {
            dbContext.CommissionRules.AddRange(
                new CommissionRule
                {
                    Id = Guid.NewGuid(),
                    MinAmount = 0,
                    MaxAmount = 4999999.99m,
                    Percentage = 2m,
                    AdminSharePercentage = 10m,
                    IsActive = true
                },
                new CommissionRule
                {
                    Id = Guid.NewGuid(),
                    MinAmount = 5000000m,
                    MaxAmount = 10000000m,
                    Percentage = 1.75m,
                    AdminSharePercentage = 10m,
                    IsActive = true
                },
                new CommissionRule
                {
                    Id = Guid.NewGuid(),
                    MinAmount = 10000000.01m,
                    MaxAmount = null,
                    Percentage = 1.5m,
                    AdminSharePercentage = 10m,
                    IsActive = true
                });

            await dbContext.SaveChangesAsync();
        }
        else
        {
            await dbContext.Database.ExecuteSqlRawAsync("""
                UPDATE [CommissionRules]
                SET [AdminSharePercentage] = 10
                WHERE [AdminSharePercentage] = 0
                """);
        }
    }

    private static async Task EnsureSchemaUpgradesAsync(ApplicationDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('CommissionRules', 'AdminSharePercentage') IS NULL
            BEGIN
                ALTER TABLE [CommissionRules] ADD [AdminSharePercentage] decimal(5,2) NOT NULL CONSTRAINT [DF_CommissionRules_AdminSharePercentage] DEFAULT 10;
            END
            """);

        await dbContext.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[PropertyBookings]', N'U') IS NULL
            BEGIN
                CREATE TABLE [PropertyBookings] (
                    [Id] uniqueidentifier NOT NULL,
                    [PropertyListingId] uniqueidentifier NOT NULL,
                    [HouseSeekerId] uniqueidentifier NOT NULL,
                    [Notes] nvarchar(1000) NOT NULL,
                    [Status] nvarchar(50) NOT NULL,
                    [AdminCommissionAmount] decimal(18,2) NOT NULL CONSTRAINT [DF_PropertyBookings_AdminCommissionAmount] DEFAULT 0,
                    [BrokerNetCommissionAmount] decimal(18,2) NOT NULL CONSTRAINT [DF_PropertyBookings_BrokerNetCommissionAmount] DEFAULT 0,
                    [CreatedAtUtc] datetime2 NOT NULL,
                    [ConfirmedAtUtc] datetime2 NULL,
                    CONSTRAINT [PK_PropertyBookings] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_PropertyBookings_AspNetUsers_HouseSeekerId] FOREIGN KEY ([HouseSeekerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_PropertyBookings_PropertyListings_PropertyListingId] FOREIGN KEY ([PropertyListingId]) REFERENCES [PropertyListings] ([Id]) ON DELETE CASCADE
                );
                CREATE INDEX [IX_PropertyBookings_HouseSeekerId] ON [PropertyBookings] ([HouseSeekerId]);
                CREATE INDEX [IX_PropertyBookings_PropertyListingId] ON [PropertyBookings] ([PropertyListingId]);
            END
            """);

        await dbContext.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('PropertyBookings', 'AdminCommissionAmount') IS NULL
            BEGIN
                ALTER TABLE [PropertyBookings] ADD [AdminCommissionAmount] decimal(18,2) NOT NULL CONSTRAINT [DF_PropertyBookings_AdminCommissionAmount_Alter] DEFAULT 0;
            END
            """);

        await dbContext.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('PropertyBookings', 'BrokerNetCommissionAmount') IS NULL
            BEGIN
                ALTER TABLE [PropertyBookings] ADD [BrokerNetCommissionAmount] decimal(18,2) NOT NULL CONSTRAINT [DF_PropertyBookings_BrokerNetCommissionAmount_Alter] DEFAULT 0;
            END
            """);

        await dbContext.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('PropertyBookings', 'ConfirmedAtUtc') IS NULL
            BEGIN
                ALTER TABLE [PropertyBookings] ADD [ConfirmedAtUtc] datetime2 NULL;
            END
            """);

        await dbContext.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[ErrorInfos]', N'U') IS NULL
            BEGIN
                CREATE TABLE [ErrorInfos] (
                    [Id] uniqueidentifier NOT NULL,
                    [Message] nvarchar(4000) NOT NULL,
                    [ExceptionType] nvarchar(500) NOT NULL,
                    [StackTrace] nvarchar(max) NULL,
                    [Path] nvarchar(1000) NOT NULL,
                    [Method] nvarchar(20) NOT NULL,
                    [UserId] nvarchar(100) NULL,
                    [DeviceId] nvarchar(200) NULL,
                    [IpAddress] nvarchar(100) NULL,
                    [CreatedAtUtc] datetime2 NOT NULL,
                    CONSTRAINT [PK_ErrorInfos] PRIMARY KEY ([Id])
                );
            END
            """);

        await dbContext.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[AppActivities]', N'U') IS NULL
            BEGIN
                CREATE TABLE [AppActivities] (
                    [Id] uniqueidentifier NOT NULL,
                    [Path] nvarchar(1000) NOT NULL,
                    [Method] nvarchar(20) NOT NULL,
                    [StatusCode] int NOT NULL,
                    [EndpointName] nvarchar(1000) NULL,
                    [UserId] nvarchar(100) NULL,
                    [DeviceId] nvarchar(200) NULL,
                    [IpAddress] nvarchar(100) NULL,
                    [CreatedAtUtc] datetime2 NOT NULL,
                    CONSTRAINT [PK_AppActivities] PRIMARY KEY ([Id])
                );
            END
            """);
    }
}
