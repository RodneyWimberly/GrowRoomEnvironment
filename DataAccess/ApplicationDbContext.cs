using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private const string systemUserId = "11111111-1111-1111-1111-111111111111";
        private string _currentUserId;
        public string CurrentUserId
        {
            get => string.IsNullOrEmpty(_currentUserId) ? systemUserId : _currentUserId;
            set => _currentUserId = value;
        }
        public DbSet<ExtendedLog> Logs { get; set; }
        public DbSet<ActionDevice> ActionDevices { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCondition> EventConditions { get; set; }
        public DbSet<DataPoint> DataPoints { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public void DetachAllEntities()
        {
            List<EntityEntry> changedEntriesCopy = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
          => options.UseSqlite("Data Source=./GrowRoomEnvironment.db")
            .ConfigureWarnings(b => b.Ignore(new EventId[] { RelationalEventId.AmbientTransactionWarning }));

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ExtendedLog
            LogModelBuilderHelper.Build(builder.Entity<ExtendedLog>());
            builder.Entity<ExtendedLog>().Property(r => r.Id).ValueGeneratedOnAdd();
            builder.Entity<ExtendedLog>().HasIndex(r => r.TimeStamp).HasName("IX_Log_TimeStamp");
            builder.Entity<ExtendedLog>().HasIndex(r => r.EventId).HasName("IX_Log_EventId");
            builder.Entity<ExtendedLog>().HasIndex(r => r.Level).HasName("IX_Log_Level");
            builder.Entity<ExtendedLog>().Property(u => u.Browser).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.User).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Host).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Path).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Method).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.StatusCode).HasColumnType("INTEGER");
            builder.Entity<ExtendedLog>().Property(u => u.ServerVariables).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Cookies).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.FormVariables).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.QueryString).HasMaxLength(255);
            builder.Entity<ExtendedLog>().ToTable($"App{nameof(this.Logs)}");

            // ApplicationUser
            builder.Entity<ApplicationUser>().Property(u => u.Id).HasColumnType("TEXT");
            builder.Entity<ApplicationUser>().HasKey(u => u.Id);
            builder.Entity<ApplicationUser>().Property(u => u.AccessFailedCount).HasColumnType("INTEGER");
            builder.Entity<ApplicationUser>().Property(u => u.Email).HasColumnType("TEXT").HasMaxLength(256);
            builder.Entity<ApplicationUser>().Property(u => u.EmailConfirmed).HasColumnType("INTEGER");
            builder.Entity<ApplicationUser>().Property(u => u.LockoutEnabled).HasColumnType("INTEGER");
            builder.Entity<ApplicationUser>().Property(u => u.LockoutEnd).HasColumnType("TEXT");
            builder.Entity<ApplicationUser>().Property(u => u.NormalizedEmail).HasColumnType("TEXT").HasMaxLength(256);
            builder.Entity<ApplicationUser>().HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");
            builder.Entity<ApplicationUser>().Property(u => u.NormalizedUserName).HasColumnType("TEXT").HasMaxLength(256);
            builder.Entity<ApplicationUser>().HasIndex(u => u.NormalizedUserName).IsUnique().HasName("UserNameIndex");
            builder.Entity<ApplicationUser>().Property(u => u.PasswordHash).HasColumnType("TEXT");
            builder.Entity<ApplicationUser>().Property(u => u.PhoneNumber).HasColumnType("TEXT");
            builder.Entity<ApplicationUser>().Property(u => u.PhoneNumberConfirmed).HasColumnType("INTEGER");
            builder.Entity<ApplicationUser>().Property(u => u.SecurityStamp).HasColumnType("TEXT");
            builder.Entity<ApplicationUser>().Property(u => u.TwoFactorEnabled).HasColumnType("INTEGER");
            builder.Entity<ApplicationUser>().Property(u => u.UserName).HasColumnType("TEXT").HasMaxLength(256);
            //builder.Entity<ApplicationUser>().Property(u => u.FriendlyName).HasMaxLength(255);
            builder.Entity<ApplicationUser>().Property(u => u.JobTitle).HasMaxLength(255);
            builder.Entity<ApplicationUser>().Property(u => u.FullName).HasMaxLength(255);
            builder.Entity<ApplicationUser>().Property(u => u.Configuration).HasMaxLength(255);
            builder.Entity<ApplicationUser>().Property(u => u.IsEnabled).HasColumnType("INTEGER");
            builder.Entity<ApplicationUser>().SetupAuditableEntityProperties();
            builder.Entity<ApplicationUser>().SetupConcurrencyTrackingEntityProperties();
            builder.Entity<ApplicationUser>().HasMany(u => u.Claims)
                .WithOne()
                .HasForeignKey(c => c.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationRole
            builder.Entity<ApplicationRole>().Property(u => u.Id).HasColumnType("TEXT");
            builder.Entity<ApplicationRole>().HasKey(u => u.Id);
            builder.Entity<ApplicationRole>().Property(u => u.Name).HasColumnType("TEXT").HasMaxLength(256);
            builder.Entity<ApplicationRole>().Property(u => u.NormalizedName).HasColumnType("TEXT").HasMaxLength(256);
            builder.Entity<ApplicationRole>().HasIndex(u => u.NormalizedName).IsUnique().HasName("RoleNameIndex");
            builder.Entity<ApplicationRole>().Property(u => u.Description).HasMaxLength(255);
            builder.Entity<ApplicationRole>().SetupAuditableEntityProperties();
            builder.Entity<ApplicationRole>().SetupConcurrencyTrackingEntityProperties();
            builder.Entity<ApplicationRole>()
                .HasMany(r => r.Claims)
                .WithOne()
                .HasForeignKey(c => c.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>()
                .HasMany(r => r.Users)
                .WithOne()
                .HasForeignKey(r => r.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // ActionDevice
            builder.Entity<ActionDevice>().Property(e => e.ActionDeviceId).IsRequired().HasColumnType("INTEGER").ValueGeneratedOnAdd();
            builder.Entity<ActionDevice>().HasIndex(e => e.ActionDeviceId).IsUnique();
            builder.Entity<ActionDevice>().HasKey(e => e.ActionDeviceId);
            builder.Entity<ActionDevice>().Property(e => e.Type).IsRequired();
            builder.Entity<ActionDevice>().HasIndex(e => e.Type);
            builder.Entity<ActionDevice>().Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Entity<ActionDevice>().Property(e => e.Parameters).IsRequired().HasMaxLength(200);
            builder.Entity<ActionDevice>().Property(e => e.IsEnabled).IsRequired().HasColumnType("INTEGER");
            builder.Entity<ActionDevice>().SetupAuditableEntityProperties();
            builder.Entity<ActionDevice>().SetupConcurrencyTrackingEntityProperties();
            builder.Entity<ActionDevice>().ToTable($"App{nameof(this.ActionDevices)}");
            builder.Entity<ActionDevice>().HasMany(e => e.Events)
                .WithOne(e => e.ActionDevice)
                .HasForeignKey(e => e.EventId)
                .IsRequired()
                .OnDelete(DeleteBehavior.SetNull);

            // DataPoint
            builder.Entity<DataPoint>().Property(e => e.DataPointId).IsRequired().HasColumnType("INTEGER").ValueGeneratedOnAdd();
            builder.Entity<DataPoint>().HasIndex(e => e.DataPointId).IsUnique();
            builder.Entity<DataPoint>().HasKey(e => e.DataPointId);
            builder.Entity<DataPoint>().Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().Property(e => e.IsEnabled).IsRequired().HasColumnType("INTEGER");
            builder.Entity<DataPoint>().SetupAuditableEntityProperties();
            builder.Entity<DataPoint>().SetupConcurrencyTrackingEntityProperties();
            builder.Entity<DataPoint>().ToTable($"App{nameof(this.DataPoints)}");
            builder.Entity<DataPoint>().HasMany(e => e.EventConditions)
                .WithOne(e => e.DataPoint)
                .HasForeignKey(e => e.EventConditionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Event
            builder.Entity<Event>().Property(e => e.EventId).IsRequired().HasColumnType("INTEGER").ValueGeneratedOnAdd();
            builder.Entity<Event>().HasIndex(e => e.EventId).IsUnique();
            builder.Entity<Event>().HasKey(e => e.EventId);
            builder.Entity<Event>().Property(e => e.State).IsRequired();
            builder.Entity<Event>().HasIndex(e => e.State);
            builder.Entity<Event>().Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Entity<Event>().Property(e => e.ActionDeviceId).IsRequired();
            builder.Entity<Event>().HasIndex(e => e.ActionDeviceId);
            builder.Entity<Event>().Property(e => e.IsEnabled).IsRequired().HasColumnType("INTEGER");
            builder.Entity<Event>().SetupAuditableEntityProperties();
            builder.Entity<Event>().SetupConcurrencyTrackingEntityProperties();
            builder.Entity<Event>().ToTable($"App{nameof(this.Events)}");
            builder.Entity<Event>().HasMany(e => e.EventConditions)
                .WithOne(e => e.Event)
                .HasPrincipalKey(e => e.EventId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Event>().HasOne(e => e.ActionDevice)
                .WithMany(e => e.Events)
                .HasForeignKey(e => e.ActionDeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // EventCondition
            builder.Entity<EventCondition>().Property(e => e.EventConditionId).IsRequired().HasColumnType("INTEGER").ValueGeneratedOnAdd();
            builder.Entity<EventCondition>().HasIndex(e => e.EventConditionId).IsUnique();
            builder.Entity<EventCondition>().HasKey(e => e.EventConditionId);
            builder.Entity<EventCondition>().Property(e => e.EventId).IsRequired();
            builder.Entity<EventCondition>().HasIndex(e => e.EventId);
            builder.Entity<EventCondition>().Property(e => e.DataPointId).IsRequired();
            builder.Entity<EventCondition>().HasIndex(e => e.DataPointId);
            builder.Entity<EventCondition>().Property(e => e.Operator).IsRequired();
            builder.Entity<EventCondition>().Property(e => e.Value).IsRequired().HasMaxLength(100);
            builder.Entity<EventCondition>().SetupAuditableEntityProperties();
            builder.Entity<EventCondition>().SetupConcurrencyTrackingEntityProperties();
            builder.Entity<EventCondition>().ToTable($"App{nameof(this.EventConditions)}");
            builder.Entity<EventCondition>().HasOne(e => e.Event)
               .WithMany(e => e.EventConditions)
               .HasForeignKey(e => e.EventId)
               .IsRequired()
               .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<EventCondition>().HasOne(e => e.DataPoint)
               .WithMany(e => e.EventConditions)
               .HasForeignKey(e => e.DataPointId)
               .IsRequired()
               .OnDelete(DeleteBehavior.SetNull);


            // Notification
            builder.Entity<Notification>().Property(e => e.NotificationId).IsRequired().HasColumnType("INTEGER").ValueGeneratedOnAdd();
            builder.Entity<Notification>().HasIndex(e => e.NotificationId).IsUnique();
            builder.Entity<Notification>().HasKey(e => e.NotificationId);
            builder.Entity<Notification>().Property(e => e.Header).IsRequired().HasMaxLength(100);
            builder.Entity<Notification>().Property(e => e.Body).IsRequired().HasMaxLength(250);
            builder.Entity<Notification>().Property(e => e.IsRead);
            builder.Entity<Notification>().Property(e => e.IsPinned);
            builder.Entity<Notification>().Property(e => e.Date);
            builder.Entity<Notification>().SetupAuditableEntityProperties();
            builder.Entity<Notification>().SetupConcurrencyTrackingEntityProperties();
            builder.Entity<Notification>().ToTable($"App{nameof(this.Notifications)}");
        }

        public override int SaveChanges()
        {
            UpdateAuditEntities();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void UpdateAuditEntities()
        {
            IEnumerable<EntityEntry> modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is ApplicationEntityBase &&
                (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (EntityEntry entry in modifiedEntries)
            {
                ApplicationEntityBase entity = (ApplicationEntityBase)entry.Entity;
                DateTime now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                    entity.CreatedBy = CurrentUserId;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }

                entity.UpdatedDate = now;
                entity.UpdatedBy = CurrentUserId;

            }
        }
    }
}
