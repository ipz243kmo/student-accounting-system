using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateOnly Date { get; set; }
}
