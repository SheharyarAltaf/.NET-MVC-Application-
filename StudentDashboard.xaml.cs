using AWS.ModelsEAD; // Ensure you have this using directive
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Frontend
{
    public partial class StudentDashboard : Window
    {
        private List<Class> availableClasses; // Use the correct Class model from AWS.ModelsEAD
        private List<Attendance> attendanceData;
        private int currentStudentId; // Store the current student's ID

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
                new Attendance { ClassName = "Science 101", TotalClasses = 30, AttendedClasses = 25 },
                new Attendance { ClassName = "Islamiyat 201", TotalClasses = 30, AttendedClasses = 20 },
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
            Class selectedClass = button?.DataContext as Class;

            if (selectedClass != null)
            {
                using (var context = new AwsContext())
                {
                    // Check if the student is already registered for the selected class
                    bool isAlreadyRegistered = context.Registereds
                        .Any(r => r.StudentId == currentStudentId && r.ClassId == selectedClass.Id);

                    if (isAlreadyRegistered)
                    {
                        MessageBox.Show($"You are already registered for {selectedClass.ClassName}.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Exit the method if already registered
                    }

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

        // Data models
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
}