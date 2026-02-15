using System;
using System.Windows;
using WpfMessageBox = System.Windows.MessageBox;
namespace StudentAccounting
{
    public partial class EditCourseWindow : Window
    {
        public string CourseName { get; private set; }
        public string Faculty { get; private set; }
        public string Department { get; private set; }
        public int StudentsCount { get; private set; }

        public EditCourseWindow(string currentName, string currentFaculty, string currentDepartment, int currentStudents)
        {
            InitializeComponent();

            // Заповнюємо поля існуючими даними
            CourseNameTextBox.Text = currentName;
            FacultyTextBox.Text = currentFaculty;
            DepartmentTextBox.Text = currentDepartment;
            StudentsCountTextBox.Text = currentStudents.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка та збереження даних
            if (!int.TryParse(StudentsCountTextBox.Text, out int count))
            {
                WpfMessageBox.Show("Кількість студентів повинна бути числом!");
                return;
            }

            CourseName = CourseNameTextBox.Text;
            Faculty = FacultyTextBox.Text;
            Department = DepartmentTextBox.Text;
            StudentsCount = count;

            // Тут можна додати логіку для оновлення БД

            WpfMessageBox.Show("Курс збережено!");
            this.DialogResult = true; // закриває вікно
        }
    }
}
