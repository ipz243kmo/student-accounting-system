using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using WpfMessageBox = System.Windows.MessageBox;

namespace StudentAccounting
{
    public partial class FacultyAddWindow : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        private List<string> departments = new();

        public FacultyAddWindow()
        {
            InitializeComponent();
            DepartmentsDataGrid.ItemsSource = departments;
        }

        private void AddDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DepartmentTextBox.Text))
            {
                departments.Add(DepartmentTextBox.Text);
                DepartmentsDataGrid.Items.Refresh();
                DepartmentTextBox.Clear();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FacultyNameTextBox.Text))
            {
                WpfMessageBox.Show("Введіть назву факультету");
                return;
            }

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();

            try
            {
                // 1️⃣ Додаємо факультет з автоматичним faculty_id
                SqlCommand facultyCmd = new SqlCommand(@"
                    DECLARE @newFacultyId INT;
                    SET @newFacultyId = ISNULL((SELECT MAX(faculty_id) FROM Faculties), 0) + 1;

                    INSERT INTO Faculties (faculty_id, name, dean)
                    VALUES (@newFacultyId, @name, NULL);

                    SELECT @newFacultyId;
                ", conn, tran);

                facultyCmd.Parameters.AddWithValue("@name", FacultyNameTextBox.Text);

                int facultyId = (int)facultyCmd.ExecuteScalar();

                // 2️⃣ Додаємо кафедри з автоматичним department_id
                foreach (string dep in departments)
                {
                    SqlCommand depCmd = new SqlCommand(@"
                        DECLARE @newDepId INT;
                        SET @newDepId = ISNULL((SELECT MAX(department_id) FROM Departments), 0) + 1;

                        INSERT INTO Departments (department_id, name, faculty_id)
                        VALUES (@newDepId, @name, @fid);
                    ", conn, tran);

                    depCmd.Parameters.AddWithValue("@name", dep);
                    depCmd.Parameters.AddWithValue("@fid", facultyId);

                    depCmd.ExecuteNonQuery();
                }

                tran.Commit();
                DialogResult = true;
            }
            catch (SqlException ex)
            {
                tran.Rollback();
                WpfMessageBox.Show("Помилка при збереженні: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
