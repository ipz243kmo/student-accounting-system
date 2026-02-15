using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using WpfMessageBox = System.Windows.MessageBox;
namespace StudentAccounting
{
    public partial class AddTeacherWindow : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        public class FacultyItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }

        public AddTeacherWindow()
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
                using SqlDataReader r = cmd.ExecuteReader();

                var faculties = new List<FacultyItem>();
                while (r.Read())
                {
                    faculties.Add(new FacultyItem
                    {
                        Id = r.GetInt32(0),
                        Name = r.IsDBNull(1) ? "" : r.GetString(1)
                    });
                }

                FacultyComboBox.ItemsSource = faculties;
            }
            catch (SqlException ex)
            {
                WpfMessageBox.Show($"Помилка завантаження факультетів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string position = PositionTextBox.Text.Trim();
            string subject = SubjectTextBox.Text.Trim();
            int? facultyId = (FacultyComboBox.SelectedItem as FacultyItem)?.Id;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(position) ||
                string.IsNullOrEmpty(subject) || facultyId == null)
            {
                WpfMessageBox.Show("Будь ласка, заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string insertQuery = @"
                    INSERT INTO Professors (FirstName, LastName, Email, Position, Subject, FacultyId)
                    VALUES (@FirstName, @LastName, @Email, @Position, @Subject, @FacultyId)";

                using SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.Parameters.AddWithValue("@Subject", subject);
                cmd.Parameters.AddWithValue("@FacultyId", facultyId.Value);

                cmd.ExecuteNonQuery();

                WpfMessageBox.Show("Викладача успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true; // закриваємо вікно
            }
            catch (SqlException ex)
            {
                    WpfMessageBox.Show($"Помилка БД: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
