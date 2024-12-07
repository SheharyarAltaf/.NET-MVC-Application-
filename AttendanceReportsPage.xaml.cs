using AWS.ModelsEAD;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Frontend
{
    public partial class AttendanceReportsPage : Window
    {
        private ObservableCollection<AttendanceRecord> attendanceRecords;

        public AttendanceReportsPage()
        {
            InitializeComponent();

            // Initialize sample data
            InitializeAttendanceData();

            LoadClasses(); // Load classes from the database
            attendanceRecords = new ObservableCollection<AttendanceRecord>(); // Initialize the collection
            attendanceDataGrid.ItemsSource = attendanceRecords;
            // Populate class dropdown
            var classNames = attendanceRecords.Select(r => r.ClassName).Distinct().ToList();
            classDropdown.ItemsSource = classNames;
        }

        private async void LoadClasses()
        {
            using (var context = new AwsContext())
            {
                var classNames = await context.Classes
                    .Select(c => c.ClassName)
                    .ToListAsync();

                classDropdown.ItemsSource = classNames;
            }
        }
        // Initialize sample attendance data
        private void InitializeAttendanceData()
        {
            attendanceRecords = new ObservableCollection<AttendanceRecord>
                {
                    new AttendanceRecord { ClassName = "Class A", Date = DateTime.Today.AddDays(-1), StudentID = "S001", StudentName = "Alice", AttendanceStatus = "Present" },
                    new AttendanceRecord { ClassName = "Class A", Date = DateTime.Today.AddDays(-1), StudentID = "S002", StudentName = "Bob", AttendanceStatus = "Absent" },
                    new AttendanceRecord { ClassName = "Class B", Date = DateTime.Today, StudentID = "S003", StudentName = "Charlie", AttendanceStatus = "Present" },
                };

            attendanceDataGrid.ItemsSource = attendanceRecords;
        }

        // Filter attendance records
        private async void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected class name
            string? selectedClass = classDropdown.SelectedItem as string;
            DateOnly? startDate = startDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(startDatePicker.SelectedDate.Value) : (DateOnly?)null;
            DateOnly? endDate = endDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(endDatePicker.SelectedDate.Value) : (DateOnly?)null;
            string studentName = studentNameFilter.Text;

            // Check if a class is selected
            if (string.IsNullOrEmpty(selectedClass))
            {
                MessageBox.Show("Please select a class.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Clear existing records
            attendanceRecords.Clear();

            try
            {
                using (var context = new AwsContext())
                {
                    // Build the query
                    var query = context.Attendances
                        .Include(a => a.Class) // Include Class navigation property
                        .Include(a => a.Student) // Include Student navigation property
                        .Where(a => a.Class.ClassName == selectedClass);

                    // Apply date filters if provided
                    if (startDate.HasValue)
                    {
                        query = query.Where(a => a.Date >= startDate.Value); // Compare DateOnly directly
                    }

                    if (endDate.HasValue)
                    {
                        query = query.Where(a => a.Date <= endDate.Value); // Compare DateOnly directly
                    }

                    // Apply student name filter if provided
                    if (!string.IsNullOrEmpty(studentName))
                    {
                        query = query.Where(a => a.Student.Name.Contains(studentName));
                    }

                    // Execute the query and select the results
                    var filteredRecords = await query
                        .Select(a => new AttendanceRecord
                        {
                            ClassName = a.Class.ClassName,
                            Date = a.Date.ToDateTime(new TimeOnly(0, 0)), // Convert DateOnly to DateTime for display
                            StudentID = a.Student.Id.ToString(),
                            StudentName = a.Student.Name,
                            AttendanceStatus = a.Status
                        })
                        .ToListAsync();

                    // Add the filtered records to the ObservableCollection
                    foreach (var record in filteredRecords)
                    {
                        attendanceRecords.Add(record);
                    }
                }

                // Update the DataGrid's ItemsSource
                attendanceDataGrid.ItemsSource = attendanceRecords;
            }
            catch (Exception ex)
            {
                // Handle exceptions and show an error message
                MessageBox.Show($"An error occurred while filtering records: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }        // Export attendance records to CSV
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var filteredRecords = (IEnumerable<AttendanceRecord>)attendanceDataGrid.ItemsSource;

            if (filteredRecords == null || !filteredRecords.Any())
            {
                MessageBox.Show("No records to export.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Save to CSV
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "AttendanceRecords.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                StringBuilder csvContent = new StringBuilder();
                csvContent.AppendLine("Class,Date,Student ID,Student Name,Attendance");

                foreach (var record in filteredRecords)
                {
                    csvContent.AppendLine($"{record.ClassName},{record.Date:MM/dd/yyyy},{record.StudentID},{record.StudentName},{record.AttendanceStatus}");
                }

                File.WriteAllText(saveFileDialog.FileName, csvContent.ToString());
                MessageBox.Show("Records exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to DashboardHomePage
            var dashboardPage = new DashboardHomePage();
            dashboardPage.Show();
            this.Close();
        }
    }

    public class AttendanceRecord
    {
        public required string ClassName { get; set; }
        public DateTime Date { get; set; }
        public required string StudentID { get; set; }
        public required string StudentName { get; set; }
        public required string AttendanceStatus { get; set; }
    }

    

}
