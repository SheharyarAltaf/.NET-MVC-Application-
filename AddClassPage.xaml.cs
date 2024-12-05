using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AWS;
using AWS.ModelsEAD;

namespace Frontend
{
    public partial class AddClassPage : Window
    {
        // ObservableCollection to store class details for DataGrid
        private ObservableCollection<ClassDetails> classList;

        public AddClassPage()
        {
            InitializeComponent();
            classList = new ObservableCollection<ClassDetails>();
            classDataGrid.ItemsSource = classList; // Bind DataGrid to ObservableCollection
            LoadClasses(); // Load data from the database when the page is initialized
        }

        // Method to Load Data from the Classes Table
        private void LoadClasses()
        {
            try
            {
                using (var context = new AWS.ModelsEAD.AwsContext())
                {
                    var classes = context.Classes.Select(c => new ClassDetails
                    {
                        ClassName = c.ClassName,
                        NumberOfStudents = c.NumStudents,
                        Created = c.Created
                    }).ToList();

                    // Clear existing data and load new data
                    classList.Clear();
                    foreach (var classDetails in classes)
                    {
                        classList.Add(classDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching classes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Button Click Event to Add Class
        private void btnAddClass_Click(object sender, RoutedEventArgs e)
        {
            //Validate Inputs
            if (string.IsNullOrWhiteSpace(txtClassName.Text) ||
                string.IsNullOrWhiteSpace(txtNumStudents.Text) ||
                string.IsNullOrWhiteSpace(datePicker.Text))
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtNumStudents.Text, out int numStudents))
            {
                MessageBox.Show("Number of Students must be a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create Class Entity
            var newClass = new AWS.ModelsEAD.Class
            {
                ClassName = txtClassName.Text,
                NumStudents = numStudents.ToString(),
                Created = datePicker.SelectedDate ?? DateTime.Now,
                TeacherId = LoggedInUser.TeacherId  // Set this to the actual teacher's ID as per your logic
            };


            // Add Class to Database
            try
            {
                using (var context = new AWS.ModelsEAD.AwsContext())
                {
                    context.Classes.Add(newClass);
                    context.SaveChanges();
                }

                // Reload DataGrid with Updated Data
                LoadClasses();

                MessageBox.Show("Class added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear Input Fields
                txtClassName.Clear();
                txtNumStudents.Clear();
                datePicker.SelectedDate = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Navigate Back to Dashboard
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var dashboardPage = new DashboardHomePage(); // Replace with actual dashboard page
            dashboardPage.Show();
            this.Close();
        }
    }

    // Class Model for DataGrid Display
    public class ClassDetails
    {
        public required string ClassName { get; set; }
        public required string NumberOfStudents { get; set; }
        public DateTime Created { get; set; }
    }
}
