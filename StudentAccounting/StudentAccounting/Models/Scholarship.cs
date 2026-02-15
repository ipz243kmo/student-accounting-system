using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Scholarship
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Amount { get; set; }

    public DateOnly AwardedOn { get; set; }

    public virtual Student Student { get; set; } = null!;
}
