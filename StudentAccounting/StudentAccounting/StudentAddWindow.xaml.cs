using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace StudentAccounting
{
    public partial class StudentAddWindow : Window
    {
        private readonly string connectionString;

        public StudentAddWindow(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;

            LoadStudyForms();
            LoadFundingOptions();
            LoadFaculties();
            LoadDepartments();
        }

        private void LoadStudyForms()
        {
            StudyFormComboBox.ItemsSource = new List<string> { "Очна", "Заочна", "Електронна" };
        }

        private void LoadFundingOptions()
        {
            FundingComboBox.ItemsSource = new List<string> { "Бюджет", "Контракт" };
        }

        private void LoadFaculties()
        {
            var faculties = new List<FacultyItem>();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT faculty_id, name FROM Faculties ORDER BY name";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    faculties.Add(new FacultyItem
                    {
                        Id = r.GetInt32(0),
                        Name = r.GetString(1)
                    });
                }
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show("Помилка завантаження факультетів: " + ex.Message);
            }

            FacultyComboBox.ItemsSource = faculties;
            FacultyComboBox.DisplayMemberPath = "Name";
            FacultyComboBox.SelectedValuePath = "Id";
        }

        private void LoadDepartments()
        {
            var departments = new List<DepartmentItem>();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT department_id, name FROM Departments ORDER BY name";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    departments.Add(new DepartmentItem
                    {
                        Id = r.GetInt32(0),
                        Name = r.GetString(1)
                    });
                }
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show("Помилка завантаження департаментів: " + ex.Message);
            }

            DepartmentComboBox.ItemsSource = departments;
            DepartmentComboBox.DisplayMemberPath = "Name";
            DepartmentComboBox.SelectedValuePath = "Id";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
INSERT INTO Students
(first_name, last_name, StudyForm, Funding, [Group], faculty_id, department_id, enrollment_date, graduation_date)
VALUES (@fn, @ln, @sf, @fund, @grp, @fac, @dep, @enroll, @grad)";

                using SqlCommand cmd = new SqlCommand(query, conn);

                // Беремо дані з текстових полів та ComboBox
                cmd.Parameters.AddWithValue("@fn", FirstNameTextBox.Text);
                cmd.Parameters.AddWithValue("@ln", LastNameTextBox.Text);
                cmd.Parameters.AddWithValue("@sf", StudyFormComboBox.SelectedItem?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@fund", FundingComboBox.SelectedItem?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@grp", GroupTextBox.Text);
                cmd.Parameters.AddWithValue("@fac", FacultyComboBox.SelectedValue ?? 0);
                cmd.Parameters.AddWithValue("@dep", DepartmentComboBox.SelectedValue ?? 0);
                cmd.Parameters.AddWithValue("@enroll", EnrollmentDatePicker.SelectedDate ?? DateTime.Now);
                cmd.Parameters.AddWithValue("@grad", GraduationDatePicker.SelectedDate ?? DateTime.Now.AddYears(4));

                

                cmd.ExecuteNonQuery();
                System.Windows.MessageBox.Show("Студента додано!");
                this.DialogResult = true;
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show("Помилка БД: " + ex.Message);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

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
    }
}
