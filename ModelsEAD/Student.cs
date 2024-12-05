using System;
using System.Collections.Generic;

namespace AWS.ModelsEAD;

public partial class Student
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Registered> Registereds { get; set; } = new List<Registered>();
}
