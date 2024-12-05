using AWS.ModelsEAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frontend
{
    public partial class StudentDashboard : Window
    {
        private List<AWS.ModelsEAD.Class> availableClasses; // Use the correct namespace
        private List<Attendance> attendanceData;
        private int currentStudentId; // Store the current student's ID

        public StudentDashboard()
        {
            InitializeComponent();
        }
        public StudentDashboard(int studentId)
        {
            InitializeComponent();
            currentStudentId = studentId; // Set the current student's ID

            using (var context = new AwsContext())
            {
                // Fetch available classes from the database
                availableClasses = context.Classes.ToList();
            }

            // Sample attendance data
            attendanceData = new List<Attendance>
            {
                new Attendance { ClassName = "Math 101", TotalClasses = 30, AttendedClasses = 25 },
                new Attendance { ClassName = "History 201", TotalClasses = 30, AttendedClasses = 20 },
                new Attendance { ClassName = "Science 301", TotalClasses = 30, AttendedClasses = 28 }
            };

            // Bind attendance data to ListView
            var attendanceList = attendanceData.Select(a => new AttendanceViewModel
            {
                ClassName = a.ClassName,
                AttendancePercentage = (a.AttendedClasses / (double)a.TotalClasses) * 100
            }).ToList();
            AttendanceList.ItemsSource = attendanceList;

            // Bind available classes to ListView for registration
            AvailableClassesList.ItemsSource = availableClasses;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AWS.ModelsEAD.Class selectedClass = button?.DataContext as AWS.ModelsEAD.Class; // Use the correct namespace

            if (selectedClass != null)
            {
                using (var context = new AwsContext())
                {
                    // Create a new registration
                    var registration = new Registered
                    {
                        ClassId = selectedClass.Id,
                        StudentId = currentStudentId // Use the current student's ID
                    };

                    context.Registereds.Add(registration);
                    context.SaveChanges();
                }

                MessageBox.Show($"You have successfully registered for {selectedClass.ClassName}!", "Registration Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
    // Data models
    public class Class
    {
        public string ClassName { get; set; }
    }

    public class Attendance
    {
        public string ClassName { get; set; }
        public int TotalClasses { get; set; }
        public int AttendedClasses { get; set; }
    }

    public class AttendanceViewModel
    {
        public string ClassName { get; set; }
        public double AttendancePercentage { get; set; }
    }
}
