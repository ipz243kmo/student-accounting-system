using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int? Grade1 { get; set; }

    public DateOnly? ExamDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
