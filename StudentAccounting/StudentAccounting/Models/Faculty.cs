using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string Name { get; set; } = null!;

    public string Dean { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
