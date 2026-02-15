using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static StudentAccounting.Admin;
using MessageBox = System.Windows.MessageBox;
namespace StudentAccounting
{
    public partial class EditTeacherWindow : Window
    {
        private Teacher teacher;
        private string connectionString = "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        public EditTeacherWindow(Teacher t, List<FacultyItem> faculties)
        {
            InitializeComponent();
            teacher = t;

            // Заповнюємо поля даними викладача
            FirstNameTextBox.Text = teacher.FirstName;
            LastNameTextBox.Text = teacher.LastName;
            PositionTextBox.Text = teacher.Position;
            SubjectTextBox.Text = teacher.Subject;

            // Заповнюємо ComboBox факультетів
            FacultyComboBox.ItemsSource = faculties;
            FacultyComboBox.DisplayMemberPath = "Name";
            FacultyComboBox.SelectedValuePath = "Id";

            var facultyItem = faculties.FirstOrDefault(f => f.Name == teacher.Faculty);
            if (facultyItem != null)
                FacultyComboBox.SelectedValue = facultyItem.Id;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Оновлюємо дані викладача у БД
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            using SqlCommand cmd = new SqlCommand(@"
                UPDATE Professors
                SET FirstName=@fn, LastName=@ln, Position=@pos, Subject=@sub, DepartmentId=@dept
                WHERE Id=@id", conn);

            cmd.Parameters.AddWithValue("@fn", FirstNameTextBox.Text);
            cmd.Parameters.AddWithValue("@ln", LastNameTextBox.Text);
            cmd.Parameters.AddWithValue("@pos", PositionTextBox.Text);
            cmd.Parameters.AddWithValue("@sub", SubjectTextBox.Text);
            cmd.Parameters.AddWithValue("@dept", (FacultyComboBox.SelectedItem as FacultyItem)?.Id ?? 0);
            cmd.Parameters.AddWithValue("@id", teacher.Id);

            cmd.ExecuteNonQuery();

            MessageBox.Show("Дані викладача оновлено!");
            this.DialogResult = true; // Повертаємо true для підтвердження
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
