using AWS;
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
    /// <summary>
    /// Interaction logic for DashboardHomePage.xaml
    /// </summary>
    /// 

    public partial class DashboardHomePage : Window
    {
        public DashboardHomePage()
        {
            InitializeComponent();
            LoadTeacherDetails();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                using (var context = new AwsContext())
                {
                    // Fetch total classes
                    int totalClasses = context.Classes.Count();

                    // Fetch total students
                    int totalStudents = context.Students.Count();

                    // Fetch attendance percentage (assuming you have an Attendance table with a Status column)
                    int totalAttendances = context.Attendances.Count(a => a.Status == "Present");
                    int totalPossibleAttendances = context.Attendances.Count();
                    double attendancePercentage = (totalAttendances / (double)totalPossibleAttendances) * 100;

                    // Update the TextBlocks
                    TotalClassesTextBlock.Text = totalClasses.ToString();
                    TotalStudentsTextBlock.Text = totalStudents.ToString();
                    AttendancePercentageTextBlock.Text = attendancePercentage.ToString("F2") + "%";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}");
            }
        }
        private void LoadTeacherDetails()
        {
            // Fetch teacher name, fallback to "Guest" if null
            string teacherName = LoggedInUser.TeacherName ?? "Guest";

            // Update the TextBlock directly
            WelcomeTextBlock.Text = $" Hey {teacherName} WELCOME, !";
        }


        // Method to fetch teacher name from database (mocked for simplicity)
        //private string FetchTeacherNameFromDatabase(int? teacherId)
        //{
        //    if (teacherId == null)
        //        return "Guest";

        //    try
        //    {
        //        using (var context = new AwsContext()) // Assume Entity Framework is used
        //        {
        //            var teacher = context.Teachers.FirstOrDefault(t => t.Id == teacherId);
        //            return teacher?.Name;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error fetching teacher details: {ex.Message}");
        //        return "Guest";
        //    }
        //}

        private void NavigateTo(string pageName)
        {
            // Logic to navigate to specific pages
            switch (pageName)
            {
                case "addclass":
                    var addClassPage = new AddClassPage();
                    addClassPage.Show();
                    this.Close();
                    break;
                case "takeattendance":
                    var attendancePage = new TakeAttendancePage();
                    attendancePage.Show();
                    this.Close();
                    break;
                case "viewreports":
                    var reportsPage = new AttendanceReportsPage();
                    reportsPage.Show();
                    this.Close();
                    break;
                case "logout":
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                    break;
            }
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string content = button.Content.ToString().Replace(" ", "").ToLower();

                NavigateTo(content.Replace(" ", ""));
            }
        }
    }
}