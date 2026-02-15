using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentAccounting.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Migration> Migrations { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Scholarship> Scholarships { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    public virtual DbSet<Timetable> Timetables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=University;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__20D6A968C1648ADB");

            entity.ToTable("Attendance");

            entity.Property(e => e.AttendanceId).HasColumnName("attendance_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3214EC07F14C7D8C");

            entity.Property(e => e.Building).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => e.DepartmentId, "idx_courses_department");

            entity.HasIndex(e => e.Name, "idx_courses_name");

            entity.Property(e => e.CourseId)
                .ValueGeneratedNever()
                .HasColumnName("course_id");
            entity.Property(e => e.Credits).HasColumnName("credits");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Courses_Departments");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_AddFacultyIfNotExist");
                    tb.HasTrigger("trg_DeleteEmptyFaculty");
                });

            entity.HasIndex(e => e.FacultyId, "idx_departments_faculty");

            entity.Property(e => e.DepartmentId)
                .ValueGeneratedNever()
                .HasColumnName("department_id");
            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_Faculties");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__6D24AA7AA29B9DF9");

            entity.HasIndex(e => e.CourseId, "idx_enrollments_course");

            entity.HasIndex(e => e.StudentId, "idx_enrollments_student");

            entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EnrollmentDate).HasColumnName("enrollment_date");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Courses");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Students");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasIndex(e => e.Name, "idx_faculties_name");

            entity.Property(e => e.FacultyId)
                .ValueGeneratedNever()
                .HasColumnName("faculty_id");
            entity.Property(e => e.Dean)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("dean");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades___3A8F732C9438BE5A");

            entity.ToTable("Grades_", tb => tb.HasTrigger("trg_Grades_CheckRange"));

            entity.HasIndex(e => e.CourseId, "idx_grades_course");

            entity.HasIndex(e => e.StudentId, "idx_grades_student");

            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.ExamDate).HasColumnName("exam_date");
            entity.Property(e => e.Grade1).HasColumnName("grade");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Courses");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Students");
        });

        modelBuilder.Entity<Migration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Migratio__3214EC07C977F773");

            entity.HasIndex(e => e.FileName, "UQ_Migrations_FileName").IsUnique();

            entity.Property(e => e.AppliedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.Hash).HasMaxLength(64);
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Professo__3214EC07D036DCF2");

            entity.HasIndex(e => e.Email, "UQ__Professo__A9D10534CEC0D758").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);

            entity.HasOne(d => d.Department).WithMany(p => p.Professors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Professors_Department");
        });

        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Scholars__3214EC07C21A4AF1");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Student).WithMany(p => p.Scholarships)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Scholarships_Student");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__2A33069A0A8F8305");

            entity.HasIndex(e => e.FacultyId, "idx_students_faculty");

            entity.HasIndex(e => e.LastName, "idx_students_lastname");

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Courses)
                .IsUnicode(false)
                .HasColumnName("courses");
            entity.Property(e => e.DateOfBirthday).HasColumnName("date_of_birthday");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EnrollmentDate).HasColumnName("enrollment_date");
            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.GraduationDate).HasColumnName("graduation_date");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone_number");

            entity.HasOne(d => d.Department).WithMany(p => p.Students)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Students_Departments");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Students)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Faculties");
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StudentCourses");

            entity.Property(e => e.Courses)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("courses");
            entity.Property(e => e.FirstName)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
        });

        modelBuilder.Entity<Timetable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Timetabl__3214EC073B91459E");

            entity.ToTable("Timetable");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Classroom).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_Classroom");

            entity.HasOne(d => d.Course).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_Course");

            entity.HasOne(d => d.Professor).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_Professor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
