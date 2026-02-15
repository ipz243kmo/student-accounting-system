using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using WpfMessageBox = System.Windows.MessageBox;

namespace StudentAccounting
{
    public partial class AddCourseWindow : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        public class FacultyItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }

        public class DepartmentItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }

        public AddCourseWindow()
        {
            InitializeComponent();
            LoadFaculties();
        }

        private void LoadFaculties()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT faculty_id, name FROM Faculties ORDER BY name";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                var faculties = new List<FacultyItem>();
                while (reader.Read())
                {
                    faculties.Add(new FacultyItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1)
                    });
                }

                FacultyComboBox.ItemsSource = faculties;
                FacultyComboBox.SelectedIndex = -1; // спочатку нічого не вибрано
            }
            catch (SqlException ex)
            {
                WpfMessageBox.Show($"Помилка завантаження факультетів: {ex.Message}");
            }
        }

        private void FacultyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FacultyComboBox.SelectedValue is int facultyId)
            {
                LoadDepartments(facultyId);
            }
        }

        private void LoadDepartments(int facultyId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT department_id, name FROM Departments WHERE faculty_id = @FacultyId ORDER BY name";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FacultyId", facultyId);

                using SqlDataReader reader = cmd.ExecuteReader();
                var departments = new List<DepartmentItem>();
                while (reader.Read())
                {
                    departments.Add(new DepartmentItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1)
                    });
                }

                DepartmentComboBox.ItemsSource = departments;
                DepartmentComboBox.SelectedIndex = -1; // спочатку нічого не вибрано
            }
            catch (SqlException ex)
            {
                WpfMessageBox.Show($"Помилка завантаження кафедр: {ex.Message}");
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CourseNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(StudentsCountTextBox.Text) ||
                FacultyComboBox.SelectedValue == null ||
                DepartmentComboBox.SelectedValue == null)
            {
                WpfMessageBox.Show("Будь ласка, заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(StudentsCountTextBox.Text, out int studentsCount))
            {
                WpfMessageBox.Show("Кількість студентів має бути числом!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string insertQuery = @"
    INSERT INTO Courses (name,  department_id)
    VALUES (@Name,  @DepartmentId)";

                using SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@Name", CourseNameTextBox.Text.Trim());
      
                cmd.Parameters.AddWithValue("@DepartmentId", (DepartmentComboBox.SelectedItem as DepartmentItem)!.Id);

                cmd.ExecuteNonQuery();


                cmd.ExecuteNonQuery();
                WpfMessageBox.Show("Курс успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true; // закриваємо вікно
            }
            catch (SqlException ex)
            {
                WpfMessageBox.Show($"Помилка БД: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
