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
            if (textBox != null && textBox.Text == "Student ID")
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
                textBox.Text = "Student ID";
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
            if (string.IsNullOrWhiteSpace(StudentIDTextBox.Text) || StudentIDTextBox.Text == "Student ID")
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

                    // Proceed with password reset logic here
                    MessageBox.Show("Password reset successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }




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

    }
}
