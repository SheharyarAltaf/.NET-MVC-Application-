using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using AWS;
using AWS.ModelsEAD;

namespace Frontend
{
    public partial class TakeAttendancePage : Window
    {
        private ObservableCollection<StudentViewModel> studentList = new ObservableCollection<StudentViewModel>();

        public TakeAttendancePage()
        {
            InitializeComponent();
            // Load classes into the dropdown when the page is initialized
            LoadClasses();
        }

        // Load class names from the Classes table into the dropdown
        private void LoadClasses()
        {
            using (var dbContext = new AwsContext())
            {
                var classes = dbContext.Classes
                    .Select(c => new { c.Id, c.ClassName })
                    .ToList();

                classDropdown.ItemsSource = classes;
                classDropdown.DisplayMemberPath = "ClassName";
                classDropdown.SelectedValuePath = "Id";
            }
        }

        // Handle class selection change
        private void ClassDropdown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (classDropdown.SelectedValue != null)
            {
                int selectedClassId = (int)classDropdown.SelectedValue;
                LoadStudentsForClass(selectedClassId);
            }
        }

        // Load students for the selected class
        private void LoadStudentsForClass(int classId)
        {
            //using (var dbContext = new AwsContext())
            //{
            //    var studentsInClass = dbContext.Students
            //        .Where(s => s.Classes.Any(c => c.Id == classId)) // Make sure students are part of the selected class
            //        .Select(s => new StudentViewModel
            //        {
            //            StudentID = s.Id,
            //            StudentName = s.Name,
            //            IsPresent = false
            //        })
            //        .ToList();

            //    studentList.Clear(); // Clear existing list
            //    foreach (var student in studentsInClass)
            //    {
            //        studentList.Add(student);
            //    }

            //    studentDataGrid.ItemsSource = studentList; // Bind to DataGrid
            //}
        }

        // Save attendance records to the Attendance table
        private void btnSaveAttendance_Click(object sender, RoutedEventArgs e)
        {
            if (classDropdown.SelectedValue == null || datePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a class and date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int selectedClassId = (int)classDropdown.SelectedValue;
            DateOnly selectedDate = DateOnly.FromDateTime(datePicker.SelectedDate.Value);

            using (var dbContext = new AwsContext())
            {
                foreach (var student in studentList)
                {
                    var attendance = new AWS.ModelsEAD.Attendance
                    {
                        ClassId = selectedClassId,
                        StudentId = student.StudentID,
                        Date = selectedDate,
                        Status = student.IsPresent ? "Present" : "Absent"
                    };

                    dbContext.Attendances.Add(attendance); // Add attendance to the database
                }

                dbContext.SaveChanges(); // Commit changes
            }

            MessageBox.Show("Attendance saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var dashboardPage = new DashboardHomePage();
            dashboardPage.Show();
            this.Close();
        }
    }

    // ViewModel for Student DataGrid
    public class StudentViewModel
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public bool IsPresent { get; set; }
    }
}
