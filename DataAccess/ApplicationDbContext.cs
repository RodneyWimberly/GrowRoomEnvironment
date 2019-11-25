using GrowRoomEnvironment.Contracts.DataAccess;
using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public string CurrentUserId { get; set; }
       // public DbSet<ElevatorStatusHistory> ElevatorStatusHistories { get; set; }
        public DbSet<DataPoint> DataPoints { get; set; }
        public DbSet<EnumLookup> EnumLookups { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)  { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=./GrowRoomEnvironment-Dev.db");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

           /* builder.Entity<ElevatorStatusHistory>().Property(e => e.Id).IsRequired().HasColumnType("int");
            builder.Entity<ElevatorStatusHistory>().HasIndex(e => e.Id);
            builder.Entity<ElevatorStatusHistory>().HasKey(e => e.Id);
            builder.Entity<ElevatorStatusHistory>().Property(e => e.ElevatorId).IsRequired().HasColumnType("smallint");
            builder.Entity<ElevatorStatusHistory>().Property(e => e.CurrentFloor).IsRequired().HasColumnType("smallint");
            builder.Entity<ElevatorStatusHistory>().Property(e => e.OriginFloor).IsRequired().HasColumnType("smallint");
            builder.Entity<ElevatorStatusHistory>().Property(e => e.DestinationFloor).IsRequired().HasColumnType("smallint");
            builder.Entity<ElevatorStatusHistory>().Property(e => e.TimeStamp).IsRequired().HasColumnType("datetime");
            builder.Entity<ElevatorStatusHistory>().Property(e => e.OperationState).IsRequired().HasColumnType("smallint");
            builder.Entity<ElevatorStatusHistory>().ToTable($"App{nameof(this.ElevatorStatusHistories)}");
            */
            builder.Entity<DataPoint>().Property(e => e.Id).IsRequired().HasColumnType("INTEGER");
            builder.Entity<DataPoint>().HasIndex(e => e.Id);
            builder.Entity<DataPoint>().HasKey(e => e.Id);
            builder.Entity<DataPoint>().Property(e => e.Caption).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().Property(e => e.Icon).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().Property(e => e.Template).IsRequired().HasMaxLength(100);
            builder.Entity<DataPoint>().Property(e => e.ShowInUI ).IsRequired().HasColumnType("INTEGER");
            builder.Entity<DataPoint>().ToTable($"App{nameof(this.DataPoints)}");
            
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
