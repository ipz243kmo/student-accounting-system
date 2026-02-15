using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentAccounting
{
    public class DepartmentItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public partial class FacultyEditWindow : Window
    {
        private readonly string connectionString;
        private readonly int facultyId;
        private List<DepartmentItem> departments = new();

        public FacultyEditWindow(int facultyId, string facultyName, string connectionString)
        {
            InitializeComponent();

            this.facultyId = facultyId;
            this.connectionString = connectionString;

            FacultyNameTextBox.Text = facultyName;

            LoadDepartments();
        }

        // Завантажуємо кафедри з БД
        private void LoadDepartments()
        {
            departments.Clear();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = "SELECT department_id, name FROM Departments WHERE faculty_id = @id";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", facultyId);

            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                departments.Add(new DepartmentItem
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1)
                });
            }

            DepartmentsDataGrid.ItemsSource = departments;
        }

        // Збереження факультету та кафедр
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // Оновлюємо назву факультету
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE Faculties SET name=@name WHERE faculty_id=@id", conn))
            {
                cmd.Parameters.AddWithValue("@name", FacultyNameTextBox.Text);
                cmd.Parameters.AddWithValue("@id", facultyId);
                cmd.ExecuteNonQuery();
            }

            // Оновлюємо існуючі та додаємо нові кафедри
            foreach (var d in departments)
            {
                if (d.Id > 0) // існуюча кафедра
                {
                    using SqlCommand cmd = new SqlCommand(
                        "UPDATE Departments SET name=@name WHERE department_id=@id", conn);
                    cmd.Parameters.AddWithValue("@name", d.Name);
                    cmd.Parameters.AddWithValue("@id", d.Id);
                    cmd.ExecuteNonQuery();
                }
                else if (!string.IsNullOrWhiteSpace(d.Name)) // нова кафедра
                {
                    using SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Departments (faculty_id, name) VALUES (@fid, @name)", conn);
                    cmd.Parameters.AddWithValue("@fid", facultyId);
                    cmd.Parameters.AddWithValue("@name", d.Name);
                    cmd.ExecuteNonQuery();
                }
            }

            DialogResult = true;
        }

        // Скасування
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        // Оновлюємо список кафедр після редагування рядка
        private void DepartmentsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Використовуємо Dispatcher, щоб оновити список після завершення редагування
                Dispatcher.InvokeAsync(() =>
                {
                    departments = DepartmentsDataGrid.Items.OfType<DepartmentItem>().ToList();
                });
            }
        }

    }
}
