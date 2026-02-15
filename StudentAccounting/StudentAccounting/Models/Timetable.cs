using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Timetable
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int ProfessorId { get; set; }

    public int ClassroomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual Professor Professor { get; set; } = null!;
}
