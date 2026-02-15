using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Classroom
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Building { get; set; }

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
