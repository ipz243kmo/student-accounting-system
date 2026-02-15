using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class StudentCourse
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Courses { get; set; }
}
