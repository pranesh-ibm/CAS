using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CAS.Models;

public partial class CasContext : DbContext
{
    public CasContext()
    {
    }

    public CasContext(DbContextOptions<CasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Chemist> Chemists { get; set; }

    public virtual DbSet<Drug> Drugs { get; set; }

    public virtual DbSet<DrugRequest> DrugRequests { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<PhysicianAdvice> PhysicianAdvices { get; set; }

    public virtual DbSet<PhysicianPrescrip> PhysicianPrescrips { get; set; }

    public virtual DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }

    public virtual DbSet<PurchaseProductLine> PurchaseProductLines { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CAS;Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA25D4FAF2D");

            entity.ToTable("Appointment");

            entity.HasIndex(e => e.PatientId, "IDX_Appointment_PatientID");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.Criticality).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.ScheduleStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Patient");
        });

        modelBuilder.Entity<Chemist>(entity =>
        {
            entity.HasKey(e => e.ChemistId).HasName("PK__Chemist__C0D5B7B415F01C53");

            entity.ToTable("Chemist");

            entity.Property(e => e.ChemistId).HasColumnName("ChemistID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ChemistName).HasMaxLength(100);
            entity.Property(e => e.ChemistStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Summary).HasMaxLength(500);
        });

        modelBuilder.Entity<Drug>(entity =>
        {
            entity.HasKey(e => e.DrugId).HasName("PK__Drug__908D66F680539923");

            entity.ToTable("Drug");

            entity.Property(e => e.DrugId).HasColumnName("DrugID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Dosage).HasMaxLength(50);
            entity.Property(e => e.DrugStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DrugTitle).HasMaxLength(100);
        });

        modelBuilder.Entity<DrugRequest>(entity =>
        {
            entity.HasKey(e => e.DrugRequestId).HasName("PK__DrugRequ__AEE9D650C71EAAC7");

            entity.ToTable("DrugRequest");

            entity.Property(e => e.DrugRequestId).HasColumnName("DrugRequestID");
            entity.Property(e => e.DrugsInfoText).HasMaxLength(500);
            entity.Property(e => e.PhysicianId).HasColumnName("PhysicianID");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Physician).WithMany(p => p.DrugRequests)
                .HasForeignKey(d => d.PhysicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DrugRequest_Physician");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC3464C9D9AEF");

            entity.ToTable("Patient");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.PatientName).HasMaxLength(100);
            entity.Property(e => e.PatientStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Summary).HasMaxLength(500);
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.PhysicianId).HasName("PK__Physicia__DFF5ED732E596253");

            entity.ToTable("Physician");

            entity.Property(e => e.PhysicianId).HasColumnName("PhysicianID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PhysicianName).HasMaxLength(100);
            entity.Property(e => e.PhysicianStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Specialization).HasMaxLength(100);
            entity.Property(e => e.Summary).HasMaxLength(500);
        });

        modelBuilder.Entity<PhysicianAdvice>(entity =>
        {
            entity.HasKey(e => e.PhysicianAdviceId).HasName("PK__Physicia__82C626109A6A8F1D");

            entity.ToTable("PhysicianAdvice");

            entity.Property(e => e.PhysicianAdviceId).HasColumnName("PhysicianAdviceID");
            entity.Property(e => e.Advice).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");

            entity.HasOne(d => d.Schedule).WithMany(p => p.PhysicianAdvices)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhysicianAdvice_Schedule");
        });

        modelBuilder.Entity<PhysicianPrescrip>(entity =>
        {
            entity.HasKey(e => e.PhysicianPrescripId).HasName("PK__Physicia__DC5A5520ACB5FEBC");

            entity.ToTable("PhysicianPrescrip");

            entity.HasIndex(e => e.DrugId, "IDX_Prescrip_DrugID");

            entity.Property(e => e.PhysicianPrescripId).HasColumnName("PhysicianPrescripID");
            entity.Property(e => e.Dosage).HasMaxLength(50);
            entity.Property(e => e.DrugId).HasColumnName("DrugID");
            entity.Property(e => e.PhysicianAdviceId).HasColumnName("PhysicianAdviceID");
            entity.Property(e => e.Prescription).HasMaxLength(255);

            entity.HasOne(d => d.Drug).WithMany(p => p.PhysicianPrescrips)
                .HasForeignKey(d => d.DrugId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescrip_Drug");

            entity.HasOne(d => d.PhysicianAdvice).WithMany(p => p.PhysicianPrescrips)
                .HasForeignKey(d => d.PhysicianAdviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescrip_Advice");
        });

        modelBuilder.Entity<PurchaseOrderHeader>(entity =>
        {
            entity.HasKey(e => e.Poid).HasName("PK__Purchase__5F02A2F4491668DC");

            entity.ToTable("PurchaseOrderHeader");

            entity.HasIndex(e => e.SupplierId, "IDX_PO_SupplierID");

            entity.HasIndex(e => e.Pono, "UQ__Purchase__5F02AA86B0C286F3").IsUnique();

            entity.Property(e => e.Poid).HasColumnName("POID");
            entity.Property(e => e.Podate).HasColumnName("PODate");
            entity.Property(e => e.Pono)
                .HasMaxLength(50)
                .HasColumnName("PONo");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrderHeaders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_POHeader_Supplier");
        });

        modelBuilder.Entity<PurchaseProductLine>(entity =>
        {
            entity.HasKey(e => e.Ppid).HasName("PK__Purchase__5FD889CD19656513");

            entity.ToTable("PurchaseProductLine");

            entity.HasIndex(e => e.Poid, "IDX_PPL_POID");

            entity.Property(e => e.Ppid).HasColumnName("PPID");
            entity.Property(e => e.DrugId).HasColumnName("DrugID");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Poid).HasColumnName("POID");

            entity.HasOne(d => d.Drug).WithMany(p => p.PurchaseProductLines)
                .HasForeignKey(d => d.DrugId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PPL_Drug");

            entity.HasOne(d => d.Po).WithMany(p => p.PurchaseProductLines)
                .HasForeignKey(d => d.Poid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PPL_PO");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B69C14C1BA7");

            entity.ToTable("Schedule");

            entity.HasIndex(e => e.PhysicianId, "IDX_Schedule_PhysicianID");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.PhysicianId).HasColumnName("PhysicianID");
            entity.Property(e => e.ScheduleStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Appointment).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Appointment");

            entity.HasOne(d => d.Physician).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.PhysicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Physician");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666947104C895");

            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
            entity.Property(e => e.SupplierStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC1CB9B425");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "UQ__User__C9F284565CF9C210").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.RoleReferenceId).HasColumnName("RoleReferenceID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
