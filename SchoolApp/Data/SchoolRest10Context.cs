using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;

namespace SchoolApp.Data;

public class SchoolRest10Context : DbContext
{
    public SchoolRest10Context(DbContextOptions<SchoolRest10Context> options) : base(options)
    { }

    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Capability> Capabilities { get; set; }
    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>(entity =>
        {

            entity.Property(e => e.Institution).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(t => t.User)
                .WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Teachers_Users_UserId");

            entity.HasIndex(e => e.Institution, "IX_Teachers_Institution");
            entity.HasIndex(e => e.UserId, "UQ_Teachers_UserId").IsUnique();
        });


        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.Am).HasMaxLength(10);
            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.Institution).HasMaxLength(50);

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Students_Users_UserId");

            entity.HasIndex(e => e.Am, "UQ_Students_Am").IsUnique();
            entity.HasIndex(e => e.Institution, "IX_Students_Institution");
            entity.HasIndex(e => e.UserId, "UQ_Students_UserId").IsUnique();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(50);

            entity.HasOne(d => d.Teacher)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Courses_Teachers_TeacherId");

            // Τα πεδία του Composite Key ονομάζονται by default StudentsId και CoursesId
            entity.HasMany(d => d.Students).WithMany(p => p.Courses).UsingEntity("StudentsCourses");

            // Αν θέλουμε explicit ονομασίες πεδίων:
            //entity.HasMany(d => d.Students).WithMany(p => p.Courses)
            //    .UsingEntity("StudentsCourses",
            //        l => l.HasOne(typeof(Student)).WithMany().HasForeignKey("StudentId"),
            //        r => r.HasOne(typeof(Course)).WithMany().HasForeignKey("CourseId"));

            entity.HasIndex(e => e.Description, "IX_Courses_Description");
            entity.HasIndex(e => e.TeacherId, "IX_Courses_TeacherId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Capabilities).WithMany(p => p.Roles)
                .UsingEntity("RolesCapabilities", j =>
                {
                    j.HasIndex("CapabilitiesId").HasDatabaseName("IX_RolesCapabilities_CapabilityId");
                });

            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();
        });

        modelBuilder.Entity<Capability>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name, "UQ_Capabilities_Name").IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(60);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(60);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Users_Roles_RoleId");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");
            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();
        });

    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(ct);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken ct = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, ct);
    }

    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                //case EntityState.Added:
                //    entry.Entity.InsertedAt = now;
                //    entry.Entity.ModifiedAt = now;
                //    break;

                //case EntityState.Modified:
                //    entry.Entity.ModifiedAt = now;
                //    break;

                //case EntityState.Deleted:
                //    entry.State = EntityState.Unchanged;
                //    entry.Entity.IsDeleted = true;
                //    entry.Entity.DeletedAt = now;
                //    entry.Entity.ModifiedAt = now;
                //    break;
            }
        }
    }
}
