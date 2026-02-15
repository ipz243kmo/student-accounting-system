using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public int FacultyId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
