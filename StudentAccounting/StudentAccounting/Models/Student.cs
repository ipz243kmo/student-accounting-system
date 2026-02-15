using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? DateOfBirthday { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public DateOnly? EnrollmentDate { get; set; }

    public DateOnly? GraduationDate { get; set; }

    public int FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public string? Courses { get; set; }

    public bool IsActive { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Scholarship> Scholarships { get; set; } = new List<Scholarship>();
}
