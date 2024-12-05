using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using AWS;
using AWS.ModelsEAD;
using System.Windows.Controls;
using System.ComponentModel;

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
            using (var dbContext = new AwsContext())
            {
                var studentsInClass = dbContext.Registereds
                    .Where(r => r.ClassId == classId)
                    .Select(r => r.Student) // Assuming `Student` is a navigation property
                    .ToList();

                studentList.Clear();
                foreach (var student in studentsInClass)
                {
                    studentList.Add(new StudentViewModel
                    {
                        StudentID = student.Id,
                        StudentName = student.Name
                    }); // Use the new constructor
                }

                studentDataGrid.ItemsSource = studentList;
            }
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

            try
            {
                using (var dbContext = new AwsContext())
                {
                    foreach (var studentViewModel in studentList)
                    {
                        if (studentViewModel == null)
                        {
                            continue; // Skip null studentViewModel
                        }

                        var attendance = new Attendance
                        {
                            ClassId = selectedClassId,
                            StudentId = studentViewModel.StudentID,
                            Date = selectedDate,
                            Status = studentViewModel.IsPresent ? "Present" : "Absent"
                        };

                        dbContext.Attendances.Add(attendance);
                    }

                    dbContext.SaveChanges(); // Commit changes
                }

                MessageBox.Show("Attendance saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving attendance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void studentDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column is DataGridCheckBoxColumn && e.Row.DataContext is StudentViewModel student)
            {
                // Get the checkbox value directly from the EditingElement
                var checkBox = e.EditingElement as CheckBox;
                if (checkBox != null)
                {
                    student.IsPresent = checkBox.IsChecked == true; // Set IsPresent based on the checkbox state
                }
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var dashboardPage = new DashboardHomePage();
            dashboardPage.Show();
            this.Close();
        }
    }

    // ViewModel for Student DataGrid
    public class StudentViewModel : INotifyPropertyChanged
    {
        private bool _isPresent;

        public int StudentID { get; set; }
        public string StudentName { get; set; }

        public bool IsPresent
        {
            get => _isPresent;
            set
            {
                if (_isPresent != value)
                {
                    _isPresent = value;
                    OnPropertyChanged(nameof(IsPresent));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}