using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;
using MessageBox = System.Windows.MessageBox;
namespace StudentAccounting
{
    public partial class AddStudentToCourseWindow : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        private readonly List<StudentItem> allStudents = new();
        private readonly List<CourseItem> allCourses = new();

        public AddStudentToCourseWindow()
        {
            InitializeComponent();
            LoadStudents();
            LoadCourses();
            LoadFacultyFilter();
        }

        #region Models
        public class StudentItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Faculty { get; set; } = "";
            public string Group { get; set; } = "";
            public string DisplayName => $"{Name} ({Faculty}, {Group})";
        }

        public class CourseItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Faculty { get; set; } = "";
            public string Department { get; set; } = "";
            public int StudentCount { get; set; }
            public string DisplayName => $"{Name} ({Faculty}) — Студентів: {StudentCount}";
        }
        #endregion

        #region Load Students
        private void LoadStudents()
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT s.student_id, s.first_name, s.last_name, f.name, s.[Group]
                FROM Students s
                JOIN Faculties f ON s.faculty_id = f.faculty_id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader r = cmd.ExecuteReader();

            allStudents.Clear();
            while (r.Read())
            {
                allStudents.Add(new StudentItem
                {
                    Id = r.GetInt32(0),
                    Name = $"{(r.IsDBNull(1) ? "" : r.GetString(1))} {(r.IsDBNull(2) ? "" : r.GetString(2))}",
                    Faculty = r.IsDBNull(3) ? "" : r.GetString(3),
                    Group = r.IsDBNull(4) ? "" : r.GetString(4)
                });
            }

            ApplyStudentFilter();
        }
        #endregion
        private void LoadCourses()
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = @"
        SELECT 
            c.course_id,
            c.name,
            c.department_id,
            COUNT(scm.student_id) AS student_count
        FROM Courses c
        LEFT JOIN StudentCourseMapping scm ON c.course_id = scm.course_id
        GROUP BY c.course_id, c.name, c.department_id
        ORDER BY c.name";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader r = cmd.ExecuteReader();

            allCourses.Clear();

            while (r.Read())
            {
                int departmentId = r.IsDBNull(2) ? 0 : r.GetInt32(2);

                allCourses.Add(new CourseItem
                {
                    Id = r.GetInt32(0),
                    Name = r.IsDBNull(1) ? "" : r.GetString(1),
                    Department = departmentId == 0 ? "" : GetDepartmentName(departmentId),
                    Faculty = departmentId == 0 ? "" : GetFacultyNameByDepartment(departmentId),
                    StudentCount = r.GetInt32(3)
                });
            }

            CoursesComboBox.ItemsSource = allCourses;
            CoursesComboBox.DisplayMemberPath = "DisplayName";
        }


        private string GetDepartmentName(int departmentId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(
                "SELECT name FROM Departments WHERE department_id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", departmentId);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        private string GetFacultyNameByDepartment(int departmentId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand(@"
                SELECT f.name
                FROM Departments d
                JOIN Faculties f ON d.faculty_id = f.faculty_id
                WHERE d.department_id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", departmentId);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }
        

        #region Filters
        private void LoadFacultyFilter()
        {
            FacultyFilterComboBox.ItemsSource = allStudents.Select(s => s.Faculty).Distinct().ToList();
        }

        private void ApplyStudentFilter()
        {
            IEnumerable<StudentItem> filtered = allStudents;

            if (FacultyFilterComboBox.SelectedItem is string faculty)
                filtered = filtered.Where(s => s.Faculty == faculty);

            if (GroupFilterComboBox.SelectedItem is string group)
                filtered = filtered.Where(s => s.Group == group);

            StudentsComboBox.ItemsSource = filtered.ToList();
        }

        private void FacultyFilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FacultyFilterComboBox.SelectedItem is string faculty)
            {
                GroupFilterComboBox.ItemsSource = allStudents
                    .Where(s => s.Faculty == faculty)
                    .Select(s => s.Group)
                    .Distinct()
                    .ToList();
            }
            ApplyStudentFilter();
        }

        private void GroupFilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyStudentFilter();
        }
        #endregion

        #region Actions
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsComboBox.SelectedItem is not StudentItem student ||
                CoursesComboBox.SelectedItem is not CourseItem course)
            {
                MessageBox.Show("Оберіть студента та курс");
                return;
            }

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            using SqlCommand check = new SqlCommand(
                "SELECT COUNT(*) FROM StudentCourseMapping WHERE student_id=@s AND course_id=@c", conn);
            check.Parameters.AddWithValue("@s", student.Id);
            check.Parameters.AddWithValue("@c", course.Id);

            if ((int)check.ExecuteScalar() > 0)
            {
                MessageBox.Show("Студент вже доданий до цього курсу");
                return;
            }

            using SqlCommand insert = new SqlCommand(
                "INSERT INTO StudentCourseMapping(student_id, course_id) VALUES(@s,@c)", conn);
            insert.Parameters.AddWithValue("@s", student.Id);
            insert.Parameters.AddWithValue("@c", course.Id);
            insert.ExecuteNonQuery();

            MessageBox.Show("Студента додано на курс");
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        #endregion
    }
}

