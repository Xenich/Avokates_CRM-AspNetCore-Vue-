using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Avokates_CRM.Models.DB
{
    public partial class LawyerCRMContext : DbContext
    {
        public LawyerCRMContext()
        {
        }

        public LawyerCRMContext(DbContextOptions<LawyerCRMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Auth> Auth { get; set; }
        public virtual DbSet<Case> Case { get; set; }
        public virtual DbSet<Classificator> Classificator { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeCase> EmployeeCase { get; set; }
        public virtual DbSet<Figurant> Figurant { get; set; }
        public virtual DbSet<FigurantRole> FigurantRole { get; set; }
        public virtual DbSet<Invite> Invite { get; set; }
        public virtual DbSet<MediaFile> MediaFile { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-IDFEOG1;Database=LawyerCRM;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auth>(entity =>
            {
                entity.Property(e => e.EmployeeUid).HasColumnName("EmployeeUID");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PassHash)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.HasOne(d => d.EmployeeU)
                    .WithMany(p => p.Auth)
                    .HasForeignKey(d => d.EmployeeUid)
                    .HasConstraintName("FK_Auth_Employee");
            });

            modelBuilder.Entity<Case>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CompanyUid).HasColumnName("CompanyUID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.CompanyU)
                    .WithMany(p => p.Case)
                    .HasForeignKey(d => d.CompanyUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Case_Company");
            });

            modelBuilder.Entity<Classificator>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ProfessionCode).HasMaxLength(20);

                entity.Property(e => e.ProfessionName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CompanyDirector).HasMaxLength(250);

                entity.Property(e => e.CompanyEmail).HasMaxLength(50);

                entity.Property(e => e.CompanyName).HasMaxLength(250);

                entity.Property(e => e.CompanyPhone).HasMaxLength(20);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.ClassificatorUid).HasColumnName("ClassificatorUID");

                entity.Property(e => e.CompanyUid).HasColumnName("CompanyUID");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.PublicKey).HasMaxLength(8000);

                entity.Property(e => e.RoleUid).HasColumnName("RoleUID");

                entity.Property(e => e.SecondName).HasMaxLength(250);

                entity.Property(e => e.Surname).HasMaxLength(250);

                entity.Property(e => e.Token).HasMaxLength(250);

                entity.HasOne(d => d.ClassificatorU)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.ClassificatorUid)
                    .HasConstraintName("FK_Employee_Classificator");

                entity.HasOne(d => d.CompanyU)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.CompanyUid)
                    .HasConstraintName("FK_Employee_Company");

                entity.HasOne(d => d.RoleU)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.RoleUid)
                    .HasConstraintName("FK_Employee_Role");
            });

            modelBuilder.Entity<EmployeeCase>(entity =>
            {
                entity.HasIndex(e => new { e.CaseUid, e.EmployeeUid })
                    .HasName("IX_EmployeeCase")
                    .IsUnique();

                entity.Property(e => e.CaseUid).HasColumnName("CaseUID");

                entity.Property(e => e.EmployeeUid).HasColumnName("EmployeeUID");

                entity.Property(e => e.EncriptedAesKey).HasMaxLength(8000);

                entity.HasOne(d => d.CaseU)
                    .WithMany(p => p.EmployeeCase)
                    .HasForeignKey(d => d.CaseUid)
                    .HasConstraintName("FK_EmployeeCase_Case");

                entity.HasOne(d => d.EmployeeU)
                    .WithMany(p => p.EmployeeCase)
                    .HasForeignKey(d => d.EmployeeUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeCase_Employee");
            });

            modelBuilder.Entity<Figurant>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CaseUid).HasColumnName("CaseUID");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FigurantRoleName).HasMaxLength(255);

                entity.Property(e => e.FigurantRoleUid).HasColumnName("FigurantRoleUID");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.SecondName).HasMaxLength(255);

                entity.Property(e => e.Surname).HasMaxLength(255);

                entity.HasOne(d => d.CaseU)
                    .WithMany(p => p.Figurant)
                    .HasForeignKey(d => d.CaseUid)
                    .HasConstraintName("FK_Figurant_Case");

                entity.HasOne(d => d.FigurantRoleU)
                    .WithMany(p => p.Figurant)
                    .HasForeignKey(d => d.FigurantRoleUid)
                    .HasConstraintName("FK_Figurant_FigurantRole");
            });

            modelBuilder.Entity<FigurantRole>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CompanyUid).HasColumnName("CompanyUID");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.CompanyU)
                    .WithMany(p => p.FigurantRole)
                    .HasForeignKey(d => d.CompanyUid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_FigurantRole_Company");
            });

            modelBuilder.Entity<Invite>(entity =>
            {
                entity.HasKey(e => e.Token);

                entity.Property(e => e.Token)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyUid).HasColumnName("CompanyUID");

                entity.Property(e => e.ExpiresIn).HasColumnType("datetime");

                entity.HasOne(d => d.CompanyU)
                    .WithMany(p => p.Invite)
                    .HasForeignKey(d => d.CompanyUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Invite_Company");
            });

            modelBuilder.Entity<MediaFile>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.FilePath).HasMaxLength(255);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.NoteUid).HasColumnName("NoteUID");

                entity.HasOne(d => d.NoteU)
                    .WithMany(p => p.MediaFile)
                    .HasForeignKey(d => d.NoteUid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_MediaFile_Note");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CaseUid).HasColumnName("CaseUID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.EmployeeUid).HasColumnName("EmployeeUID");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Updatedate).HasColumnType("datetime");

                entity.HasOne(d => d.CaseU)
                    .WithMany(p => p.Note)
                    .HasForeignKey(d => d.CaseUid)
                    .HasConstraintName("FK_Note_Case");

                entity.HasOne(d => d.EmployeeU)
                    .WithMany(p => p.Note)
                    .HasForeignKey(d => d.EmployeeUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Note_Employee");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.Property(e => e.Parameter)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Value).HasMaxLength(250);
            });
        }
    }
}
