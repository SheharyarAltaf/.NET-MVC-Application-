using AWS;
using AWS.ModelsEAD;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
//using AmsContext = AWS.AmsContext;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txt_error.Visibility = Visibility.Collapsed;
        }
        public MainWindow(string s)
        {
            InitializeComponent();
            txt_error.Visibility = Visibility.Visible;
            txt_error.Foreground = Brushes.Green;
            txt_error.Text = s; 
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                textEmail.Visibility = Visibility.Collapsed;
            }
            else
            {
                textEmail.Visibility = Visibility.Visible;
            }
        }

        //Student Name placeholders
        private void textStudentName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Clear the hint text when the TextBox gains focus
            txtStudentName.Focus();
            //textStudentName.Visibility = Visibility.Collapsed;
        }

        private void txtStudentName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Show or hide the hint text based on whether the TextBox is empty
            if (!string.IsNullOrEmpty(txtStudentName.Text))
            {
                textStudentName.Visibility = Visibility.Collapsed;
            }
            else
            {
                textStudentName.Visibility = Visibility.Visible;
            }
        }



        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password))
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Check if email and password are entered
            if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                txt_error.Visibility = Visibility.Visible;
                txt_error.Text = "Please enter a valid email address and password!";
                txt_error.Foreground = Brushes.Red;
                return;
            }

            txt_error.Visibility = Visibility.Collapsed;

            // Check if a role is selected
            if (roleComboBox.SelectedItem is ComboBoxItem selectedRole)
            {
                string? role = selectedRole.Content?.ToString();

                if (string.IsNullOrEmpty(role))
                {
                    txt_error.Visibility = Visibility.Visible;
                    txt_error.Text = "Please select a valid role!";
                    return;
                }

                using (var context = new AwsContext())
                {
                    if (role == "Student")
                    {
                        // Check the Student table
                        var student = context.Students
                            .FirstOrDefault(s => s.Email == txtEmail.Text && s.Password == txtPassword.Password);

                        if (student != null)
                        {
                            LoggedInUser.StudentId = student.Id;
                            LoggedInUser.StudentName = student.Name;
                            // Navigate to Student Dashboard
                            StudentDashboard studentDashboard = new StudentDashboard(student.Id);
                            studentDashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            txt_error.Visibility = Visibility.Visible;
                            txt_error.Text = "Invalid Student email or password.";
                            txt_error.Foreground = Brushes.Red;
                        }
                    }
                    else if (role == "Teacher")
                    {
                        // Check the Teacher table
                        var teacher = context.Teachers
                            .FirstOrDefault(t => t.Email == txtEmail.Text && t.Password == txtPassword.Password);

                        if (teacher != null)
                        {

                            LoggedInUser.TeacherId = teacher.Id;
                            LoggedInUser.TeacherName = teacher.Name;
                            // Navigate to Teacher Dashboard
                            DashboardHomePage dashboardHome = new DashboardHomePage();
                            dashboardHome.Show();
                            this.Close();
                        }
                        else
                        {
                            txt_error.Visibility = Visibility.Visible;
                            txt_error.Text = "Invalid Teacher email or password.";
                            txt_error.Foreground = Brushes.Red;
                        }
                    }
                    else
                    {
                        txt_error.Visibility = Visibility.Visible;
                        txt_error.Text = "Please select a valid role.";
                    }
                }
            }
            else
            {
                txt_error.Visibility = Visibility.Visible;
                txt_error.Text = "Please select a role.";
            }
        }



        


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Signup signUpPage = new Signup();

            // Show the sign-up window
            signUpPage.Show();

            // Optionally hide the main window if you don't want both windows open
            this.Hide();
        }

        private void ResetPasswordLink_Click(object sender, MouseButtonEventArgs e)
        {
            // Close the current window

            // Open the Forget.xaml window when the link is clicked
            Forget forgetWindow = new Forget(); // Assuming Forget.xaml is the name of your window
            forgetWindow.Show();
            this.Close();
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if roleComboBox and its selected item are not null
            if (roleComboBox.SelectedItem is ComboBoxItem selectedRole && textEmail != null && textStudentName != null)
            {
                // Get the selected role as a string
                string? role = selectedRole.Content?.ToString();

                // Update placeholders based on the selected role
                if (role == "Student")
                {
                    textEmail.Text = "Student ID";
                    textStudentName.Text = "Student Name";
                }
                else if (role == "Teacher")
                {
                    textEmail.Text = "Teacher ID";
                    textStudentName.Text = "Teacher Name";
                }
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

}
