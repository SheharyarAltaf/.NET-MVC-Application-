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
    public partial class DashboardHomePage : Window
    {
        public DashboardHomePage()
        {
            InitializeComponent();
            LoadTeacherDetails();
        }

        private void LoadTeacherDetails()
        {
            // Mock teacher name, replace with actual logic to fetch from the database or API
            string teacherName = "John Doe";

            // Update the welcome message dynamically
            foreach (var element in MainGrid.Children)
            {
                if (element is TextBlock textBlock && textBlock.Text.Contains("Welcome"))
                {
                    textBlock.Text = $"Welcome, {teacherName}";
                    break;
                }
            }
        }

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