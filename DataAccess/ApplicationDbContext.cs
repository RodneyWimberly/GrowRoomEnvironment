using GrowRoomEnvironment.Contracts.DataAccess;
using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public string CurrentUserId { get; set; }

        public DbSet<ExtendedLog> Logs { get; set; }

        public DbSet<ActionDevice> ActionDevices { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCondition> EventConditions { get; set; }
        public DbSet<DataPoint> DataPoints { get; set; }
        public DbSet<EnumLookup> EnumLookups { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)  { }

       protected override void OnConfiguring(DbContextOptionsBuilder options)
          => options.UseSqlite("Data Source=./GrowRoomEnvironment.db");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // build default model.
            LogModelBuilderHelper.Build(builder.Entity<ExtendedLog>());

            // real relation database can map table:
            builder.Entity<ExtendedLog>().Property(r => r.Id).ValueGeneratedOnAdd();
            builder.Entity<ExtendedLog>().HasIndex(r => r.TimeStamp).HasName("IX_Log_TimeStamp");
            builder.Entity<ExtendedLog>().HasIndex(r => r.EventId).HasName("IX_Log_EventId");
            builder.Entity<ExtendedLog>().HasIndex(r => r.Level).HasName("IX_Log_Level");
            builder.Entity<ExtendedLog>().Property(u => u.Name).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Browser).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.User).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Host).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Path).HasMaxLength(255);
            builder.Entity<ExtendedLog>().ToTable($"App{nameof(this.Logs)}");

            builder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ActionDevice>().Property(e => e.ActionDeviceId).IsRequired().HasColumnType("INTEGER");
            builder.Entity<ActionDevice>().HasIndex(e => e.ActionDeviceId);
            builder.Entity<ActionDevice>().HasKey(e => e.ActionDeviceId);
            builder.Entity<ActionDevice>().Property(e => e.Type).IsRequired();
            builder.Entity<ActionDevice>().Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Entity<ActionDevice>().Property(e => e.Parameters).IsRequired().HasMaxLength(200);
            builder.Entity<ActionDevice>().ToTable($"App{nameof(this.ActionDevices)}");
            builder.Entity<ActionDevice>().HasMany(e => e.Events)
                .WithOne(e => e.ActionDevice)
                .HasForeignKey(e => e.EventId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DataPoint>().Property(e => e.DataPointId).IsRequired().HasColumnType("INTEGER");
            builder.Entity<DataPoint>().HasIndex(e => e.DataPointId);
            builder.Entity<DataPoint>().HasKey(e => e.DataPointId);
            builder.Entity<DataPoint>().Property(e => e.Caption).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().Property(e => e.Icon).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().Property(e => e.Template).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().Property(e => e.ShowInUI ).IsRequired().HasColumnType("INTEGER");         
            builder.Entity<DataPoint>().Property(e => e.Template).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().ToTable($"App{nameof(this.DataPoints)}");
            builder.Entity<DataPoint>().HasMany(e => e.EventConditions)
                .WithOne(e => e.DataPoint)
                .HasPrincipalKey(e => e.DataPointId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Event>().Property(e => e.EventId).IsRequired().HasColumnType("INTEGER");
            builder.Entity<Event>().HasIndex(e => e.EventId);
            builder.Entity<Event>().HasKey(e => e.EventId);
            builder.Entity<Event>().Property(e => e.State).IsRequired();
            builder.Entity<Event>().Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Entity<Event>().Property(e => e.ActionDeviceId).IsRequired();
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

            builder.Entity<EventCondition>().Property(e => e.EventConditionId).IsRequired().HasColumnType("INTEGER");
            builder.Entity<EventCondition>().HasIndex(e => e.EventConditionId);
            builder.Entity<EventCondition>().HasKey(e => e.EventConditionId);
            builder.Entity<EventCondition>().Property(e => e.EventId).IsRequired();
            builder.Entity<EventCondition>().Property(e => e.DataPointId).IsRequired();;
            builder.Entity<EventCondition>().Property(e => e.Operator).IsRequired();
            builder.Entity<EventCondition>().Property(e => e.Value).IsRequired().HasMaxLength(100);
            builder.Entity<EventCondition>().ToTable($"App{nameof(this.EventConditions)}");
            builder.Entity<EventCondition>().HasOne(e => e.Event)
               .WithMany(e => e.EventConditions)
               .HasForeignKey(e => e.EventConditionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<EventCondition>().HasOne(e => e.DataPoint)
               .WithMany(e => e.EventConditions)
               .HasForeignKey(e => e.DataPointId)
               .IsRequired()
               .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<EnumLookup>().Property(e => e.Id).IsRequired().HasColumnType("INTEGER");
            builder.Entity<EnumLookup>().HasIndex(e => e.Id);
            builder.Entity<EnumLookup>().HasKey(e => e.Id);
            builder.Entity<EnumLookup>().Property(e => e.Table).IsRequired().HasMaxLength(100);
            builder.Entity<EnumLookup>().HasIndex(e => e.Table);
            builder.Entity<EnumLookup>().Property(e => e.EnumName).IsRequired().HasMaxLength(100);
            builder.Entity<EnumLookup>().HasIndex(e => e.EnumName);
            builder.Entity<EnumLookup>().Property(e => e.EnumValue).IsRequired().HasColumnType("INTEGER");
            builder.Entity<EnumLookup>().Property(e => e.EnumDescription).IsRequired().HasMaxLength(250);
            builder.Entity<EnumLookup>().ToTable($"App{nameof(this.EnumLookups)}");
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
            System.Collections.Generic.IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity && 
                (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in modifiedEntries)
            {
                IAuditableEntity entity = (IAuditableEntity)entry.Entity;
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
