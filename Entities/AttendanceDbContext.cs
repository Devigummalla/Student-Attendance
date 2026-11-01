using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Text;
using System.Configuration;


// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Student_Att.Entities
{
    public partial class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext()
        {
        }

        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attendance> Attendance { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<UserSessions> UserSessions { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Read the connection string from ConnectionStrings.config instead of hard - coding it
                string connectionString = ConfigurationManager.ConnectionStrings["AttendanceDb"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.AttendanceDate })
                    .HasName("UQ_Attendance_Student_Date")
                    .IsUnique();

                entity.Property(e => e.AttendanceDate).HasColumnType("date");

                entity.Property(e => e.AttendanceStatus)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MarkedByUser)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.MarkedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Attendanc__Marke__656C112C");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Attendanc__Stude__6477ECF3");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.DepartmentId)
                    .HasName("PK__Departme__B2079BEDB9AC1558");

                entity.HasIndex(e => e.DepartmentName)
                    .HasName("UQ__Departme__D949CC34111F91D7")
                    .IsUnique();

                entity.HasIndex(e => e.DeptCode)
                    .HasName("UQ__Departme__BB9B955041B9FDBA")
                    .IsUnique();

                entity.Property(e => e.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DeptCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.StudentId)
                    .HasName("PK__Students__32C52B99A302DFF5");

                entity.HasIndex(e => e.RollNumber)
                    .HasName("UQ__Students__E9F06F16AAA6381D")
                    .IsUnique();

                entity.HasIndex(e => new { e.DepartmentId, e.YearOfStudy })
                    .HasName("IX_Students_Department_Year");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RollNumber)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.StudentName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Students__Depart__5EBF139D");
            });

            modelBuilder.Entity<UserSessions>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK__UserSess__C9F492904AEA64F9");

                entity.HasIndex(e => e.TokenId)
                    .HasName("UQ__UserSess__658FEEEB8ABE2E26")
                    .IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpiresAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSessions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserSessi__UserI__5441852A");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users__1788CC4CA6A5E65B");

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__Users__A9D10534C18FE2FC")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("UQ__Users__536C85E433DB5F13")
                    .IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PinHash)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Faculty')");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
