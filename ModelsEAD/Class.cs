using System;
using System.Collections.Generic;

namespace AWS.ModelsEAD;

public partial class Class
{
    public int Id { get; set; }

    public string ClassName { get; set; } = null!;

    public int TeacherId { get; set; }

    public DateTime Created { get; set; }

    public string NumStudents { get; set; } = null!;

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Registered> Registereds { get; set; } = new List<Registered>();

    public virtual Teacher Teacher { get; set; } = null!;
}
