using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using MessageBox = System.Windows.MessageBox;

namespace StudentAccounting
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        public MainWindow()
        {
            InitializeComponent();
            LoadStudents();
        }

        // ===== Модель студента =====
        public class Student
        {
            public int StudentId { get; set; }
            public string LastName { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string FullName => $"{LastName} {FirstName}";
        }

        // ===== Завантаження студентів з БД =====
        private void LoadStudents()
        {
            var students = new List<Student>();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"SELECT student_id, last_name, first_name
                                 FROM Students
                                 WHERE Email NOT LIKE '%@university.edu'";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        StudentId = reader.GetInt32(0),
                        LastName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        FirstName = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    });
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка підключення до бази даних:\n{ex.Message}", "Помилка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Невідома помилка:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Прив'язка до ComboBox
            if (StudentsComboBox != null)
            {
                StudentsComboBox.ItemsSource = students;
                StudentsComboBox.DisplayMemberPath = "FullName"; // відображає повне ім’я
                StudentsComboBox.SelectedValuePath = "StudentId"; // можна використовувати для Id
            }
        }

        // ===== Вхід адміністратора =====
        private void AdminLoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Admin adminWindow = new Admin();
                adminWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити вікно адміністратора:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== Вхід студента =====
        private void StudentLoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsComboBox.SelectedItem is not Student student)
            {
                MessageBox.Show("Оберіть студента!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Register registerWindow = new Register(student.StudentId);
                registerWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити вікно студента:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
