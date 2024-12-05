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

            // Populate class dropdown
            var classNames = attendanceRecords.Select(r => r.ClassName).Distinct().ToList();
            classDropdown.ItemsSource = classNames;
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
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string selectedClass = classDropdown.SelectedItem as string;
            DateTime? startDate = startDatePicker.SelectedDate;
            DateTime? endDate = endDatePicker.SelectedDate;
            string studentName = studentNameFilter.Text;

            var filteredRecords = attendanceRecords.Where(record =>
                (string.IsNullOrEmpty(selectedClass) || record.ClassName == selectedClass) &&
                (!startDate.HasValue || record.Date >= startDate) &&
                (!endDate.HasValue || record.Date <= endDate) &&
                (string.IsNullOrEmpty(studentName) || record.StudentName.IndexOf(studentName, StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();

            attendanceDataGrid.ItemsSource = filteredRecords;
        }

        // Export attendance records to CSV
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
        public string ClassName { get; set; }
        public DateTime Date { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
