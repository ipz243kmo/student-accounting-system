using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Professor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int DepartmentId { get; set; }

    public string? Email { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
