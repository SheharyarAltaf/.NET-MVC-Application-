using AWS.ModelsEAD;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Security.Cryptography;
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
    /// Interaction logic for Forget.xaml
    /// </summary>
    public partial class Forget : Window
    {
        public Forget()
        {
            InitializeComponent();
        }
        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Email")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Email";
                textBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void RemoveNewPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (NewPasswordPlaceholder.Visibility == Visibility.Visible)
            {
                NewPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void AddNewPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                NewPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void RemoveConfirmPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (ConfirmPasswordPlaceholder.Visibility == Visibility.Visible)
            {
                ConfirmPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void AddConfirmPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                ConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Initially hide both error messages
            StudentIDError.Visibility = Visibility.Collapsed;
            PasswordMismatchError.Visibility = Visibility.Collapsed;

            // Check if the Student ID field is empty
            if (string.IsNullOrWhiteSpace(StudentIDTextBox.Text) || StudentIDTextBox.Text == "Email")
            {
                // Show the error message if the Student ID is not entered or still has placeholder text
                StudentIDError.Visibility = Visibility.Visible;
            }
            else
            {
                // Proceed to check if the passwords match
                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    // Show error message if passwords do not match
                    PasswordMismatchError.Visibility = Visibility.Visible;
                }
                else
                {
                    // Hide password mismatch error if passwords match
                    PasswordMismatchError.Visibility = Visibility.Collapsed;
                    try
                    {

                        // Proceed with password reset logic here
                        using (var context = new AwsContext())
                        {
                            // Check if the email exists in the Student table
                            var student = context.Students.FirstOrDefault(s => s.Email == StudentIDTextBox.Text);
                            if (student != null)
                            {
                                // Update the password (make sure to hash it if necessary)
                                student.Password = (NewPasswordBox.Password); // Implement HashPassword method
                                context.SaveChanges();

                                MainWindow mainWindow = new MainWindow("Student's Password Reset Successfully!");
                                mainWindow.Show();
                                this.Close();
                            }
                            

                            // Check if the email exists in the Teacher table
                            var teacher = context.Teachers.FirstOrDefault(t => t.Email == StudentIDTextBox.Text);
                            if (teacher != null)
                            {
                                // Update the password (make sure to hash it if necessary)
                                teacher.Password = (NewPasswordBox.Password); // Implement HashPassword method
                                context.SaveChanges();

                                MainWindow mainWindow = new MainWindow("Teacher's Password Reset Successfully!");
                                mainWindow.Show();
                                this.Close();
                            }
                            
                            
                                // If email not found in both tables

                       MessageBox.Show("Email not found in the system.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions and show an error message
                        MessageBox.Show($"An error occurred while filtering records: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                
               } 
            }
        }


        //private string HashPassword(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return Convert.ToBase64String(bytes);
        //    }
        //}



        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Check if NewPasswordBox and ConfirmPasswordBox passwords match
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                PasswordMismatchError.Visibility = Visibility.Visible;  // Show error message
            }
            else
            {
                PasswordMismatchError.Visibility = Visibility.Collapsed; // Hide error message
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); // Replace with your actual page class
            mainWindow.Show();
            this.Close();
        }
    }
}
