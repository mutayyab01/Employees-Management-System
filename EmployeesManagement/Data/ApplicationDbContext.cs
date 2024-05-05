using EmployeesManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Metrics;

namespace EmployeesManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var releationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                releationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            builder.Entity<LeaveApplication>()
                .HasOne(f => f.Status)
                .WithMany()
                .HasForeignKey(f => f.StatusId)
                .OnDelete(DeleteBehavior.Cascade);
        }



        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeDetail> SystemCodeDetails { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<SystemProfile> SystemProfiles { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }
        public DbSet<Audit> AuditLogs { get; set; }
        public DbSet<RoleProfile> RoleProfiles { get; set; }
        public DbSet<Holiday> Holidays { get; set; }

        public virtual async Task<int> SaveChangesAsync(string userid = null)
        {

            OnBeforeSavingChanges(userid);
            var result = await base.SaveChangesAsync();
            return result;
        }
        private void OnBeforeSavingChanges(string UserId)
        {
            ChangeTracker.DetectChanges();
            var AuditEntries = new List<AuditEntry>();
            foreach (var Entry in ChangeTracker.Entries())
            {
                if (Entry.Entity is Audit || Entry.State == EntityState.Detached || Entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(Entry);
                auditEntry.TableName = Entry.Entity.GetType().Name;
                auditEntry.UserId = UserId;
                AuditEntries.Add(auditEntry);

                foreach (var property in Entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;

                    }
                    switch (Entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;

                    }
                }


            }
            foreach (var auditentry in AuditEntries)
            {
                AuditLogs.Add(auditentry.ToAudit());

            }






        }
    }
}
