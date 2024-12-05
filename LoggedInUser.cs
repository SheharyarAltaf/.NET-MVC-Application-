using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS
{
    public static class LoggedInUser
    {
        // Static properties to store the logged-in teacher's details
        public static int TeacherId { get; set; }
        public static string? TeacherName { get; set; }

        public static int StudentId { get; set; }

        public static string? StudentName { get; set; }

    }
}