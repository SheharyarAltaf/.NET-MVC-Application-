using System;
using System.Collections.Generic;

namespace AWS.ModelsEAD;

public partial class Teacher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
