using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Windows;
using WpfMessageBox = System.Windows.MessageBox;

namespace StudentAccounting
{
    public partial class EditStudentWindow : Window
    {
        private readonly Admin.Student student;
        private readonly string connectionString;

        public class FacultyItem { public int Id { get; set; } public string Name { get; set; } = ""; }
        public class DepartmentItem { public int Id { get; set; } public string Name { get; set; } = ""; }

        public EditStudentWindow(Admin.Student student, string connectionString)
        {
            InitializeComponent();
            this.student = student;
            this.connectionString = connectionString;

            LoadStudyForms();
            LoadFundingOptions();
            LoadFaculties();
            LoadDepartments();

            FirstNameTextBox.Text = student.FirstName;
            LastNameTextBox.Text = student.LastName;
            GroupTextBox.Text = student.Group;
            StudyFormComboBox.SelectedItem = student.StudyForm;
            FundingComboBox.SelectedItem = student.Funding;

            if (FacultyComboBox.ItemsSource is List<FacultyItem> faculties)
                FacultyComboBox.SelectedValue = faculties.Find(f => f.Name == student.Faculty)?.Id ?? 0;

            if (DepartmentComboBox.ItemsSource is List<DepartmentItem> departments)
                DepartmentComboBox.SelectedValue = departments.Find(d => d.Name == student.Department)?.Id ?? 0;
        }

        private void LoadStudyForms() => StudyFormComboBox.ItemsSource = new List<string> { "Очна", "Заочна", "Електронна" };
        private void LoadFundingOptions() => FundingComboBox.ItemsSource = new List<string> { "Бюджет", "Контракт" };

        private void LoadFaculties()
        {
            var faculties = new List<FacultyItem>();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                using SqlCommand cmd = new SqlCommand("SELECT faculty_id, name FROM Faculties ORDER BY name", conn);
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    faculties.Add(new FacultyItem { Id = r.GetInt32(0), Name = r.GetString(1) });
            }
            catch (SqlException ex) { WpfMessageBox.Show("Помилка завантаження факультетів: " + ex.Message); }
            FacultyComboBox.ItemsSource = faculties;
        }

        private void LoadDepartments()
        {
            var departments = new List<DepartmentItem>();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                using SqlCommand cmd = new SqlCommand("SELECT department_id, name FROM Departments ORDER BY name", conn);
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    departments.Add(new DepartmentItem { Id = r.GetInt32(0), Name = r.GetString(1) });
            }
            catch (SqlException ex) { WpfMessageBox.Show("Помилка завантаження департаментів: " + ex.Message); }
            DepartmentComboBox.ItemsSource = departments;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = @"
UPDATE Students
SET first_name=@fn,
    last_name=@ln,
    [Group]=@grp,
    StudyForm=@sf,
    Funding=@fund,
    faculty_id=@fac,
    department_id=@dep
WHERE student_id=@id";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@fn", FirstNameTextBox.Text);
                cmd.Parameters.AddWithValue("@ln", LastNameTextBox.Text);
                cmd.Parameters.AddWithValue("@grp", GroupTextBox.Text);
                cmd.Parameters.AddWithValue("@sf", StudyFormComboBox.SelectedItem?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@fund", FundingComboBox.SelectedItem?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@fac", FacultyComboBox.SelectedValue ?? 0);
                cmd.Parameters.AddWithValue("@dep", DepartmentComboBox.SelectedValue ?? 0);
                cmd.Parameters.AddWithValue("@id", student.Id);
                cmd.ExecuteNonQuery();

                WpfMessageBox.Show("Дані студента оновлено!");
                this.DialogResult = true;
            }
            catch (SqlException ex) { WpfMessageBox.Show("Помилка БД: " + ex.Message); }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.DialogResult = false;
    }
}
