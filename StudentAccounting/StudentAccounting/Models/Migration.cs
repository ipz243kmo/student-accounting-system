using System;
using System.Collections.Generic;

namespace StudentAccounting.Models;

public partial class Migration
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public DateTime AppliedOn { get; set; }
}
