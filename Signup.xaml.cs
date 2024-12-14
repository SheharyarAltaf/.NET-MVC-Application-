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
using System.Windows.Shapes;
using AWS.ModelsEAD;


namespace Frontend
{
    public partial class Signup : Window
    {
        public Signup()
        {
            InitializeComponent();
            RadioButton_Checked(rbStudent, null);
            //AttachPasswordValidation();
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == "Email")
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Email";
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void RemovePlaceholder1(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == "Full Name")
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPlaceholder1(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Full Name";
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        

        private void RemovePasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_pass.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void AddPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_pass.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void RemoveConfirmPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_cpass.Password))
            {
                ConfirmPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void AddConfirmPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_cpass.Password))
            {
                ConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        //private void RoleSelection_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (StudentRadioButton.IsChecked == true)
        //    {
        //        StudentFieldsPanel.Visibility = Visibility.Visible;
        //        TeacherFieldsPanel.Visibility = Visibility.Collapsed;
        //    }
        //    else if (TeacherRadioButton.IsChecked == true)
        //    {
        //        StudentFieldsPanel.Visibility = Visibility.Collapsed;
        //        TeacherFieldsPanel.Visibility = Visibility.Visible;
        //    }
        //}

        private void RadioButton_Checked(object sender, RoutedEventArgs? e)
        {
            if (rbStudent.IsChecked == true)
            {
                // Update placeholders for Student
                SetPlaceholders(txt_email, "Student ID");
                SetPlaceholders(txt_fullname, "Student Name");
            }
            else if (rbTeacher.IsChecked == true)
            {
                // Update placeholders for Teacher
                SetPlaceholders(txt_email, "Teacher ID");
                SetPlaceholders(txt_fullname, "Teacher Name");
            }
        }

        private void SetPlaceholders(TextBox textBox, string placeholderText)
        {
            // Set placeholder text dynamically
            if (string.IsNullOrWhiteSpace(textBox.Text) ||
                textBox.Text == "Student ID" ||
                textBox.Text == "Teacher ID" ||
                textBox.Text == "Student Name" ||
                textBox.Text == "Teacher Name")
            {
                textBox.Text = placeholderText;
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }

            // Add focus handlers to dynamically manage placeholder behavior
            textBox.GotFocus += (sender, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.Foreground = System.Windows.Media.Brushes.Black;
                }
            };

            textBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.Foreground = System.Windows.Media.Brushes.Gray;
                }
            };
        }

        //private void ValidatePasswords(object sender, RoutedEventArgs e)
        //{
        //    if (txt_pass.Password != txt_cpass.Password)
        //    {
        //        txt_error.Visibility = Visibility.Visible; // Show error
        //    }
        //    else
        //    {
        //        txt_error.Visibility = Visibility.Collapsed; // Hide error
        //    }
        //}

        //private void AttachPasswordValidation()
        //{
        //    txt_pass.PasswordChanged += ValidatePasswords;
        //    txt_cpass.PasswordChanged += ValidatePasswords;
        //}

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page (e.g., DashboardPage)
            MainWindow mainWindow = new MainWindow(); // Replace with your actual page class
            mainWindow.Show();
            this.Close(); // Close the Signup page
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_email.Text) ||
                string.IsNullOrWhiteSpace(txt_fullname.Text) ||
                string.IsNullOrWhiteSpace(txt_pass.Password) ||
                string.IsNullOrWhiteSpace(txt_cpass.Password))
            {
                txt_error.Text = "Please fill all the fields.";
                return;
            }

            if (txt_pass.Password != txt_cpass.Password)
            {
                txt_error.Text = "Passwords do not match.";
                return;
            }

            try
            {
                using (var context = new AwsContext())
                {
                    // Check if the email already exists in Students or Teachers table
                    bool emailExistsInStudents = context.Students.Any(s => s.Email == txt_email.Text);
                    bool emailExistsInTeachers = context.Teachers.Any(t => t.Email == txt_email.Text);

                    if (emailExistsInStudents || emailExistsInTeachers)
                    {
                        txt_error.Text = "A user with this email already exists.";
                        return;
                    }

                    // Proceed with the registration process
                    if (rbStudent.IsChecked == true)
                    {
                        AWS.ModelsEAD.Student student = new AWS.ModelsEAD.Student
                        {
                            Email = txt_email.Text,
                            Name = txt_fullname.Text,
                            Password = txt_pass.Password
                        };

                        context.Students.Add(student);
                        context.SaveChanges();

                        MainWindow mainWindow = new MainWindow("Student Registered Successfully!");
                        mainWindow.Show();
                        this.Close();
                    }
                    else if (rbTeacher.IsChecked == true)
                    {
                        AWS.ModelsEAD.Teacher teacher = new AWS.ModelsEAD.Teacher
                        {
                            Email = txt_email.Text,
                            Name = txt_fullname.Text,
                            Password = txt_pass.Password
                        };

                        context.Teachers.Add(teacher);
                        context.SaveChanges();

                        MainWindow mainWindow = new MainWindow("Teacher Registered Successfully!");
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        txt_error.Text = "Please select a role.";
                    }
                }
            }
            catch (Exception ex)
            {
                txt_error.Text = $"An error occurred: {ex.Message}";
            }
        }


    }
}



