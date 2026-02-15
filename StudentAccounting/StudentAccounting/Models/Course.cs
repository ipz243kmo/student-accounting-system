using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string Name { get; set; } = null!;

    public int Credits { get; set; }

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
