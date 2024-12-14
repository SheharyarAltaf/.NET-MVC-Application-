using AWS.ModelsEAD; // Ensure you have this using directive
using Microsoft.EntityFrameworkCore;
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
        private int currentStudentId; // Store the current student's ID

        public StudentDashboard(int studentId)
        {
            InitializeComponent();
            currentStudentId = studentId; // Set the current student's ID

            using (var context = new AwsContext())
            {
                // Fetch available classes from the database
                availableClasses = context.Classes.ToList();

                // Fetch registered classes for the current student
                var registeredClasses = context.Registereds
                    .Where(r => r.StudentId == currentStudentId)
                    .Include(r => r.Class) // Include class details
                    .ToList();

                // Fetch attendance data for the current student
                var attendanceList = registeredClasses.Select(r => new AttendanceViewModel
                {
                    ClassName = r.Class.ClassName,
                    AttendancePercentage = CalculateAttendancePercentage(r.ClassId, currentStudentId)
                }).ToList();

                // Bind attendance data to ListView
                AttendanceList.ItemsSource = attendanceList;
            }

            // Bind available classes to ListView for registration
            AvailableClassesList.ItemsSource = availableClasses;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private double CalculateAttendancePercentage(int classId, int studentId)
        {
            using (var context = new AwsContext())
            {
                // Fetch attendance records for the given class and student
                var attendanceRecords = context.Attendances
                    .Where(a => a.ClassId == classId && a.StudentId == studentId)
                    .ToList();

                // Debugging: Log the attendance records count
                //MessageBox.Show($"Class ID: {classId}, Student ID: {studentId}, Records Count: {attendanceRecords.Count}");

                if (attendanceRecords.Count == 0)
                    return 0; // No attendance records found

                // Correctly count attended classes where Status is "Present"
                int attendedClasses = attendanceRecords.Count(a =>
                    !string.IsNullOrEmpty(a.Status) &&
                    a.Status.Trim().Equals("Present", StringComparison.OrdinalIgnoreCase)); // Ensure case-insensitive and trimmed comparison

                // Debugging: Log the attended classes count
                int totalClasses = attendanceRecords.Count;
                //MessageBox.Show($"Total Classes: {totalClasses}, Attended Classes: {attendedClasses}");

                return (attendedClasses / (double)totalClasses) * 100;
            }
        }

        private void RefreshAttendanceList()
        {
            using (var context = new AwsContext())
            {
                // Fetch registered classes for the current student
                var registeredClasses = context.Registereds
                    .Where(r => r.StudentId == currentStudentId)
                    .Include(r => r.Class) // Include class details
                    .ToList();

                // Fetch attendance data for the current student
                var attendanceList = registeredClasses.Select(r => new AttendanceViewModel
                {
                    ClassName = r.Class.ClassName,
                    AttendancePercentage = CalculateAttendancePercentage(r.ClassId, currentStudentId)
                }).ToList();

                // Bind the refreshed attendance data to ListView
                AttendanceList.ItemsSource = attendanceList;
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            Class? selectedClass = button?.DataContext as Class;

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

                // Refresh the attendance list in real time
                RefreshAttendanceList();
            }
        }

        // Data models
        public class AttendanceViewModel
        {
            public required string ClassName { get; set; }
            public double AttendancePercentage { get; set; }
        }
    }
}