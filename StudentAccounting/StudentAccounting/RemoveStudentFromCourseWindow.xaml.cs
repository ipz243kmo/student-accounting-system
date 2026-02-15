using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static StudentAccounting.AddStudentToCourseWindow;
using MessageBox = System.Windows.MessageBox;
namespace StudentAccounting
{
    public partial class RemoveStudentFromCourseWindow : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        private List<StudentItem> allStudents = new();
        private List<CourseItem> allCourses = new();

        public RemoveStudentFromCourseWindow()
        {
            InitializeComponent();
            LoadStudents();
            LoadCourses();
            LoadFaculties();
        }

        private void LoadCoursesForStudent(int studentId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
        SELECT
            c.course_id,
            c.name,
            d.name AS department_name,
            f.name AS faculty_name
        FROM StudentCourseMapping scm
        INNER JOIN Courses c ON scm.course_id = c.course_id
        INNER JOIN Departments d ON c.department_id = d.department_id
        INNER JOIN Faculties f ON d.faculty_id = f.faculty_id
        WHERE scm.student_id = @studentId
        ORDER BY c.name
    ";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            using SqlDataReader r = cmd.ExecuteReader();

            allCourses.Clear();

            while (r.Read())
            {
                allCourses.Add(new CourseItem
                {
                    Id = r.GetInt32(r.GetOrdinal("course_id")),
                    Name = r.GetString(r.GetOrdinal("name")),
                    Department = r.GetString(r.GetOrdinal("department_name")),
                    Faculty = r.GetString(r.GetOrdinal("faculty_name"))
                });
            }

            CourseComboBox.ItemsSource = null;
            CourseComboBox.ItemsSource = allCourses;
        }




        private void StudentsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsComboBox.SelectedItem is StudentItem student)
            {
                LoadCoursesForStudent(student.Id);
            }
        }


        private void LoadStudents()
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
        SELECT 
            s.student_id,
            s.first_name,
            s.last_name,
            f.name AS faculty_name,
            s.[Group]
        FROM Students s
        LEFT JOIN Departments d ON s.department_id = d.department_id
        LEFT JOIN Faculties f ON d.faculty_id = f.faculty_id
    ";

            using SqlCommand cmd = new(query, conn);
            using SqlDataReader r = cmd.ExecuteReader();

            allStudents.Clear();
            while (r.Read())
            {
                allStudents.Add(new StudentItem
                {
                    Id = r.GetInt32(0),
                    Name = $"{r.GetString(1)} {r.GetString(2)}",
                    Faculty = r.IsDBNull(3) ? "" : r.GetString(3),
                    Group = r.IsDBNull(4) ? "" : r.GetString(4)
                });
            }
        }



        private void LoadCourses()
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                SELECT course_id, name
                FROM Courses";

            using SqlCommand cmd = new(query, conn);
            using SqlDataReader r = cmd.ExecuteReader();

            allCourses.Clear();
            while (r.Read())
            {
                allCourses.Add(new CourseItem
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    StudentCount = GetStudentCountForCourse(r.GetInt32(0))
                });
            }

            CourseComboBox.ItemsSource = allCourses;
        }

        private int GetStudentCountForCourse(int courseId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string q = "SELECT COUNT(*) FROM StudentCourseMapping WHERE course_id = @id";
            using SqlCommand cmd = new(q, conn);
            cmd.Parameters.AddWithValue("@id", courseId);
            return (int)cmd.ExecuteScalar();
        }

        private void LoadFaculties()
        {
            FacultyComboBox.ItemsSource =
                allStudents.Select(s => s.Faculty)
                           .Distinct()
                           .Where(f => f != "")
                           .ToList();
        }

        // ------------------ ФІЛЬТРАЦІЯ ------------------

        private void FacultyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string faculty = FacultyComboBox.SelectedItem as string;

            GroupComboBox.ItemsSource =
                allStudents.Where(s => s.Faculty == faculty)
                           .Select(s => s.Group)
                           .Distinct()
                           .ToList();
        }

        private void GroupComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string faculty = FacultyComboBox.SelectedItem as string;
            string group = GroupComboBox.SelectedItem as string;

            StudentsComboBox.ItemsSource =
                allStudents.Where(s => s.Faculty == faculty && s.Group == group)
                           .ToList();
        }

        // ------------------ КНОПКИ ------------------

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsComboBox.SelectedItem is not StudentItem student ||
                CourseComboBox.SelectedItem is not CourseItem course)
            {
                MessageBox.Show("Оберіть студента та курс");
                return;
            }

            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                DELETE FROM StudentCourseMapping
                WHERE student_id = @studentId AND course_id = @courseId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@studentId", student.Id);
            cmd.Parameters.AddWithValue("@courseId", course.Id);

            cmd.ExecuteNonQuery();

            MessageBox.Show("Студента видалено з курсу");
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
