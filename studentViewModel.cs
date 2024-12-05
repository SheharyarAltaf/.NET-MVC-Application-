using AWS.ModelsEAD;
using System.ComponentModel;

public class StudentViewModel : INotifyPropertyChanged
{
    private bool _isPresent;

    public int StudentID { get; set; } // Correctly aligned with database model
    public string StudentName { get; set; } // Name of the student

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

    // Constructor to initialize from a Student object
    public StudentViewModel(Student student)
    {
        StudentID = student.Id;
        StudentName = student.Name;
        IsPresent = false; // Default value for attendance
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
