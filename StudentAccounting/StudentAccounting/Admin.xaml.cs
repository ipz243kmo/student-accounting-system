using Microsoft.Data.SqlClient;
using StudentAccounting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace StudentAccounting
{
    public partial class Admin : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        private List<Teacher> allTeachers = new List<Teacher>();
        private List<Student> allStudents = new List<Student>();

        private Teacher selectedTeacher = null;
        private Student selectedStudent = null;
        private bool isTeacherEditMode = false;

        public Admin()
        {
            InitializeComponent();
            LoadTeachers();
            LoadStudents();
            
            LoadPositions();
            LoadGroups();
            LoadSubjects();
            
            LoadStudentFaculties();
            LoadStudentGroups();
            LoadStudentStudyForms();
            LoadStudentFunding();
            LoadFacultiesData();
            LoadCourses();
            LoadSchedule();
            LoadGroupsFilter();

        }
        
        // ===== MODELS =====
        public class Teacher
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Name => $"{FirstName} {LastName}";
            public string Faculty { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Position { get; set; } = "";
        }
       
        public class Student
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Name => $"{FirstName} {LastName}";
            public string StudyForm { get; set; } = "";
            public string Faculty { get; set; } = "";
            public string Department { get; set; } = "";
            public string Group { get; set; } = "";
            public string Funding { get; set; } = "";
            public DateTime EnrollmentDate { get; set; }
            public DateTime GraduationDate { get; set; }
            public int StudentId => Id;
        }
        public class Course
        {
            public int Id { get; set; }
            public string CourseNumber { get; set; } // те, що відображається в DataGrid
            public string Faculty { get; set; }
            public string Department { get; set; }
            public int StudentCount { get; set; }
            public int CourseId => Id;
        }


        public class FacultyItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Dean { get; set; } = "";
            public int StudentCount { get; set; } = 0; // кількість студентів по факультету
        }


        // ===== EDIT MODE FOR TEACHER =====
      

        public class FacultyDepartmentItem
        {
            public int FacultyId { get; set; }
            public string Faculty { get; set; } = "";
            public string Department { get; set; } = "";
            public int StudentCount { get; set; } = 0;
        }

        // ===== LOAD TEACHERS =====
        private void LoadTeachers()
        {
            allTeachers.Clear();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = @"
                    SELECT p.Id, p.FirstName, p.LastName, f.name, p.Subject, p.Position
                    FROM Professors p
                    LEFT JOIN Departments d ON p.DepartmentId = d.department_id
                    LEFT JOIN Faculties f ON d.faculty_id = f.faculty_id";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    allTeachers.Add(new Teacher
                    {
                        Id = r.GetInt32(0),
                        FirstName = r.IsDBNull(1) ? "" : r.GetString(1),
                        LastName = r.IsDBNull(2) ? "" : r.GetString(2),
                        Faculty = r.IsDBNull(3) ? "" : r.GetString(3),
                        Subject = r.IsDBNull(4) ? "" : r.GetString(4),
                        Position = r.IsDBNull(5) ? "" : r.GetString(5)
                    });
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка БД: {ex.Message}");
            }
            ApplyTeacherFilter();
        }

       

        private void LoadSubjects()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                using SqlCommand cmd = new SqlCommand(
                    "SELECT DISTINCT Subject FROM Professors WHERE Subject IS NOT NULL ORDER BY Subject", conn);
                using SqlDataReader r = cmd.ExecuteReader();
                var subjects = new List<string> { "Усі" };
                while (r.Read()) subjects.Add(r.GetString(0));

                FilterSubjectComboBox.ItemsSource = subjects;
                FilterSubjectComboBox.SelectedIndex = 0;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка БД: {ex.Message}");
            }
        }
        // Клас для відображення розкладу
       

        private void LoadPositions()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                using SqlCommand cmd = new SqlCommand(
                    "SELECT DISTINCT Position FROM Professors WHERE Position IS NOT NULL ORDER BY Position", conn);
                using SqlDataReader r = cmd.ExecuteReader();
                var positions = new List<string> { "Усі" };
                while (r.Read()) positions.Add(r.GetString(0));

                FilterPositionComboBox.ItemsSource = positions;
                FilterPositionComboBox.SelectedIndex = 0;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка БД: {ex.Message}");
            }
        }

        private void TeacherFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyTeacherFilter();
        }

        private void ApplyTeacherFilter()
        {
            var filtered = allTeachers.AsEnumerable();
            
            if (FilterSubjectComboBox.SelectedItem is string s && s != "Усі")
                filtered = filtered.Where(t => t.Subject == s);
            if (FilterPositionComboBox.SelectedItem is string p && p != "Усі")
                filtered = filtered.Where(t => t.Position == p);

            TeachersDataGrid.ItemsSource = filtered.ToList();
        }

        // ===== LOAD STUDENTS =====
        private void LoadStudents()
        {
            allStudents.Clear();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
SELECT s.student_id, s.first_name, s.last_name, s.StudyForm,
       f.name AS FacultyName, d.name AS DepartmentName, s.[Group], s.Funding,
       s.enrollment_date, s.graduation_date
FROM Students s
LEFT JOIN Faculties f ON s.faculty_id = f.faculty_id
LEFT JOIN Departments d ON s.department_id = d.department_id";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    allStudents.Add(new Student
                    {
                        Id = r.GetInt32(0),
                        FirstName = r.IsDBNull(1) ? "" : r.GetString(1),
                        LastName = r.IsDBNull(2) ? "" : r.GetString(2),
                        StudyForm = r.IsDBNull(3) ? "" : r.GetString(3),
                        Faculty = r.IsDBNull(4) ? "" : r.GetString(4),
                        Department = r.IsDBNull(5) ? "" : r.GetString(5),
                        Group = r.IsDBNull(6) ? "" : r.GetString(6),
                        Funding = r.IsDBNull(7) ? "" : r.GetString(7),   // <- фінансування
                        EnrollmentDate = r.IsDBNull(8) ? DateTime.MinValue : r.GetDateTime(8),
                        GraduationDate = r.IsDBNull(9) ? DateTime.MinValue : r.GetDateTime(9)
                    });
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка БД: {ex.Message}");
            }

            StudentsDataGrid.ItemsSource = null;
            StudentsDataGrid.ItemsSource = allStudents;   // <- оновлення DataGrid
        }

        private void LoadStudentFaculties()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = "SELECT faculty_id, name FROM Faculties ORDER BY name";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader r = cmd.ExecuteReader();

                var faculties = new List<FacultyItem>();
                faculties.Add(new FacultyItem { Id = 0, Name = "Усі факультети" }); // Додати пункт "Усі"

                while (r.Read())
                {
                    faculties.Add(new FacultyItem
                    {
                        Id = r.GetInt32(0),
                        Name = r.GetString(1)
                    });
                }

                FilterFacultyComboBox.ItemsSource = faculties;
                FilterFacultyComboBox.DisplayMemberPath = "Name";
                FilterFacultyComboBox.SelectedValuePath = "Id";
                FilterFacultyComboBox.SelectedIndex = 0; // Вибрано "Усі"

                FilterFacultyComboBox.SelectionChanged += StudentFilter_Changed;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка БД: {ex.Message}");
            }
        }

        private void LoadStudentGroups()
        {
            var groups = allStudents.Select(s => s.Group).Where(g => !string.IsNullOrEmpty(g)).Distinct().OrderBy(g => g).ToList();
            groups.Insert(0, "Усі групи");
            FilterGroupComboBox.ItemsSource = groups;
            FilterGroupComboBox.SelectedIndex = 0;
            FilterGroupComboBox.SelectionChanged += StudentFilter_Changed;
        }

        private void LoadStudentStudyForms()
        {
            var forms = allStudents.Select(s => s.StudyForm).Where(f => !string.IsNullOrEmpty(f)).Distinct().OrderBy(f => f).ToList();
            forms.Insert(0, "Усі форми");
            FilterStudyFormComboBox.ItemsSource = forms;
            FilterStudyFormComboBox.SelectedIndex = 0;
            FilterStudyFormComboBox.SelectionChanged += StudentFilter_Changed;
        }

        private void LoadStudentFunding()
        {
            var fundings = allStudents.Select(s => s.Funding).Where(f => !string.IsNullOrEmpty(f)).Distinct().OrderBy(f => f).ToList();
            fundings.Insert(0, "Усі фінансування");
            FilterFinanceComboBox.ItemsSource = fundings;
            FilterFinanceComboBox.SelectedIndex = 0;
            FilterFinanceComboBox.SelectionChanged += StudentFilter_Changed;
        }
        private void LoadFacultiesData()
        {
            var list = new List<FacultyDepartmentItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1️⃣ Отримуємо всі факультети та департаменти
                    string deptQuery = @"
SELECT d.department_id, d.name AS DepartmentName, f.faculty_id, f.name AS FacultyName
FROM Departments d
JOIN Faculties f ON d.faculty_id = f.faculty_id
ORDER BY f.name, d.name";

                    var departments = new List<(int DeptId, string DeptName, int FacultyId, string FacultyName)>();

                    using (SqlCommand cmd = new SqlCommand(deptQuery, conn))
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            departments.Add((
                                r.GetInt32(0),
                                r.GetString(1),
                                r.GetInt32(2),
                                r.GetString(3)
                            ));
                        }
                    }

                    // 2️⃣ Отримуємо всіх студентів
                    string studentQuery = "SELECT department_id FROM Students";
                    var studentCounts = new Dictionary<int, int>(); // key = department_id

                    using (SqlCommand cmd = new SqlCommand(studentQuery, conn))
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            int deptId = r.GetInt32(0);
                            if (!studentCounts.ContainsKey(deptId))
                                studentCounts[deptId] = 0;
                            studentCounts[deptId]++;
                        }
                    }

                    // 3️⃣ Формуємо список для DataGrid
                    foreach (var d in departments)
                    {
                        list.Add(new FacultyDepartmentItem
                        {
                            Faculty = d.FacultyName,
                            Department = d.DeptName,
                            StudentCount = studentCounts.ContainsKey(d.DeptId) ? studentCounts[d.DeptId] : 0
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show("Помилка завантаження факультетів та департаментів: " + ex.Message);
            }

            FacultiesDataGrid.ItemsSource = list;
        }





        private void LoadGroups()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                using SqlCommand cmd = new SqlCommand(
                    "SELECT DISTINCT [Group] FROM Students WHERE [Group] IS NOT NULL", conn);
                using SqlDataReader r = cmd.ExecuteReader();

                var groups = new List<string> { "Усі" }; // додаємо пункт "Усі"
                while (r.Read()) groups.Add(r.GetString(0));

                // сортування в C#
                var sortedGroups = groups.Skip(1).OrderBy(g => g).ToList(); // сортуємо усі, крім "Усі"
                sortedGroups.Insert(0, "Усі"); // вставляємо "Усі" на початок

                GroupComboBox.ItemsSource = sortedGroups;
                GroupComboBox.SelectedIndex = 0;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка БД: {ex.Message}");
            }
        }



        // ===== SELECTION =====
       

        private void StudentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsDataGrid.SelectedItem is Student s)
            {
                selectedStudent = s;
            }
        }

        // ===== TEACHERS BUTTONS =====
        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            AddTeacherWindow w = new AddTeacherWindow { Owner = this };
            if (w.ShowDialog() == true) LoadTeachers();
        }

        private void EditTeacher_Click(object sender, RoutedEventArgs e)
        {
            // Беремо обраного викладача безпосередньо з DataGrid
            if (TeachersDataGrid.SelectedItem is not Teacher selectedTeacher)
            {
                MessageBox.Show("Оберіть викладача для редагування!");
                return;
            }

            // Завантажуємо список факультетів
            List<FacultyItem> faculties = LoadFaculties();

            // Відкриваємо вікно редагування
            var editWindow = new EditTeacherWindow(selectedTeacher, faculties);
            if (editWindow.ShowDialog() == true)
            {
                LoadTeachers(); // Оновлюємо DataGrid після редагування
            }
        }


        private void TeachersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeachersDataGrid.SelectedItem is Teacher t)
            {
                selectedTeacher = t; // ⚡ важливо!
            }
            else
            {
                selectedTeacher = null;
            }
        }


        // Метод для завантаження факультетів
        private List<FacultyItem> LoadFaculties()
        {
            var faculties = new List<FacultyItem>();

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
            return faculties;
        }





        private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTeacher == null)
            {
                MessageBox.Show("Оберіть викладача для видалення!");
                return;
            }
            if (MessageBox.Show($"Видалити {selectedTeacher.Name}?", "Підтвердження",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                using SqlCommand cmd = new SqlCommand("DELETE FROM Professors WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", selectedTeacher.Id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Викладача видалено!");
                LoadTeachers();
            }
        }

        public class CourseView
        {
            public string CourseNumber { get; set; } = "";
            public string Faculty { get; set; } = "";
            public string Department { get; set; } = "";
            public int StudentCount { get; set; }
        }

        private void LoadCourses()
        {
            var courses = new List<CourseView>();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
SELECT 
    c.name AS CourseNumber,
    f.name AS Faculty,
    d.name AS Department,
    COUNT(s.student_id) AS StudentCount
FROM Courses c
LEFT JOIN Departments d ON c.department_id = d.department_id
LEFT JOIN Faculties f ON d.faculty_id = f.faculty_id
LEFT JOIN Students s ON s.courses LIKE '%' + c.name + '%'
GROUP BY c.name, f.name, d.name
ORDER BY c.name;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    courses.Add(new CourseView
                    {
                        CourseNumber = r.IsDBNull(0) ? "" : r.GetString(0),
                        Faculty = r.IsDBNull(1) ? "" : r.GetString(1),
                        Department = r.IsDBNull(2) ? "" : r.GetString(2),
                        StudentCount = r.IsDBNull(3) ? 0 : r.GetInt32(3)
                    });
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Помилка завантаження курсів: " + ex.Message);
                return;
            }

            CoursesDataGrid.ItemsSource = courses;
        }








        public class ScheduleView
        {
            public string Day { get; set; } = "";
            public string Time { get; set; } = "";
            public string Group { get; set; } = "";// Наприклад: "09:00 - 10:30"
            public string Subject { get; set; } = "";
            public string Room { get; set; } = "";

        }

        private void LoadGroupsFilter()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand("SELECT DISTINCT [Group] FROM Schedule WHERE [Group] IS NOT NULL ORDER BY [Group]", conn);
                using SqlDataReader r = cmd.ExecuteReader();

                var groups = new List<string> { "Усі" }; // "Усі" для скидання фільтра
                while (r.Read())
                {
                    groups.Add(r.GetString(0));
                }

                GroupComboBox.ItemsSource = groups;
                GroupComboBox.SelectedIndex = 0; // за замовчуванням "Усі"
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Помилка завантаження груп: " + ex.Message);
            }
        }
        private void LoadSchedule()
        {
            var scheduleList = new List<ScheduleView>();

            string selectedGroup = GroupComboBox.SelectedItem?.ToString();
            bool filterByGroup = !string.IsNullOrEmpty(selectedGroup) && selectedGroup != "Усі";

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
SELECT 
    DayOfWeek,
    StartTime,
    EndTime,
    Course,
    Classroom,
    [Group]
FROM Schedule";

                if (filterByGroup)
                {
                    query += " WHERE [Group] = @group";
                }

                query += @"
ORDER BY 
    CASE DayOfWeek
        WHEN 'Monday' THEN 1
        WHEN 'Tuesday' THEN 2
        WHEN 'Wednesday' THEN 3
        WHEN 'Thursday' THEN 4
        WHEN 'Friday' THEN 5
        WHEN 'Saturday' THEN 6
        WHEN 'Sunday' THEN 7
        ELSE 8
    END,
    StartTime";

                using SqlCommand cmd = new SqlCommand(query, conn);
                if (filterByGroup)
                {
                    cmd.Parameters.AddWithValue("@group", selectedGroup);
                }

                using SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    string day = r.IsDBNull(0) ? "" : r.GetString(0);

                    string startTime = "";
                    if (!r.IsDBNull(1))
                    {
                        object val = r.GetValue(1);
                        startTime = val is TimeSpan ts ? ts.ToString(@"hh\:mm") : val.ToString();
                    }

                    string endTime = "";
                    if (!r.IsDBNull(2))
                    {
                        object val = r.GetValue(2);
                        endTime = val is TimeSpan ts ? ts.ToString(@"hh\:mm") : val.ToString();
                    }

                    string course = r.IsDBNull(3) ? "" : r.GetString(3);
                    string classroom = r.IsDBNull(4) ? "" : r.GetString(4);
                    string group = r.IsDBNull(5) ? "" : r.GetString(5);

                    scheduleList.Add(new ScheduleView
                    {
                        Day = day,
                        Time = $"{startTime} - {endTime}",
                        Subject = course,
                        Room = classroom,
                        Group = group
                    });
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Помилка завантаження розкладу: " + ex.Message);
                return;
            }

            ScheduleDataGrid.ItemsSource = scheduleList;
        }

       



        // ===== STUDENTS BUTTONS =====
        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            StudentAddWindow w = new StudentAddWindow(connectionString) { Owner = this };
            if (w.ShowDialog() == true)
                LoadStudents();
        }



        private void EditStudent_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsDataGrid.SelectedItem is not Student student)
            {
                MessageBox.Show("Оберіть студента для редагування!");
                return;
            }

            // Відкрити вікно редагування
            EditStudentWindow editWindow = new EditStudentWindow(student, connectionString)
            {
                Owner = this
            };

            if (editWindow.ShowDialog() == true)
            {
                LoadStudents();
                LoadStudentGroups();
                LoadStudentStudyForms();
                LoadStudentFunding();
            }
        }






        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            // Отримуємо виділеного студента прямо з DataGrid
            if (StudentsDataGrid.SelectedItem is not Student student)
            {
                MessageBox.Show("Оберіть студента для видалення!");
                return;
            }

            if (MessageBox.Show($"Видалити {student.Name}?", "Підтвердження",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();
                    using SqlCommand cmd = new SqlCommand("DELETE FROM Students WHERE student_id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", student.Id);
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Помилка БД: " + ex.Message);
                    return;
                }

                MessageBox.Show("Студента видалено!");
                LoadStudents(); // Оновлюємо список студентів
            }
        }


        // ===== FACULTY BUTTONS (заглушки) =====
        private void AddFaculty_Click(object sender, RoutedEventArgs e)
        {
            FacultyAddWindow w = new FacultyAddWindow
            {
                Owner = this
            };

            if (w.ShowDialog() == true)
            {
                // Після успішного додавання оновлюємо список факультетів
                LoadFacultiesData();
            }
        }

        private void EditFaculty_Click(object sender, RoutedEventArgs e)
        {
            if (FacultiesDataGrid.SelectedItem is not FacultyDepartmentItem selected)
            {
                MessageBox.Show("Оберіть факультет");
                return;
            }

            FacultyEditWindow w = new FacultyEditWindow(
                selected.FacultyId,
                selected.Faculty,
                connectionString
            )
            {
                Owner = this
            };

            if (w.ShowDialog() == true)
                LoadFacultiesData();   // ⬅️ дивись КРОК 4
        }


        private void DeleteFaculty_Click(object sender, RoutedEventArgs e)
        {
            // Перевіряємо, що користувач виділив рядок
            if (FacultiesDataGrid.SelectedItem is FacultyDepartmentItem selected)
            {
                // Питання користувачу про підтвердження
                var result = System.Windows.MessageBox.Show(
                    $"Видалити факультет '{selected.Faculty}' разом з усіма його департаментами?",
                    "Підтвердження",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // 1️⃣ Видаляємо студенти, які належать факультету
                            string deleteStudents = "DELETE FROM Students WHERE faculty_id = (SELECT faculty_id FROM Faculties WHERE name = @name)";
                            using (SqlCommand cmd = new SqlCommand(deleteStudents, conn))
                            {
                                cmd.Parameters.AddWithValue("@name", selected.Faculty);
                                cmd.ExecuteNonQuery();
                            }

                            // 2️⃣ Видаляємо департаменти факультету
                            string deleteDepartments = "DELETE FROM Departments WHERE faculty_id = (SELECT faculty_id FROM Faculties WHERE name = @name)";
                            using (SqlCommand cmd = new SqlCommand(deleteDepartments, conn))
                            {
                                cmd.Parameters.AddWithValue("@name", selected.Faculty);
                                cmd.ExecuteNonQuery();
                            }

                            // 3️⃣ Видаляємо сам факультет
                            string deleteFaculty = "DELETE FROM Faculties WHERE name = @name";
                            using (SqlCommand cmd = new SqlCommand(deleteFaculty, conn))
                            {
                                cmd.Parameters.AddWithValue("@name", selected.Faculty);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Оновлюємо DataGrid
                        LoadFacultiesData();
                        System.Windows.MessageBox.Show($"Факультет '{selected.Faculty}' видалено!");
                    }
                    catch (SqlException ex)
                    {
                        System.Windows.MessageBox.Show("Помилка при видаленні факультету: " + ex.Message);
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Будь ласка, виділіть факультет для видалення.");
            }
        }
        private void LoadCoursesFromDatabase()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
            SELECT c.course_id, c.name, c.StudentsCount, f.name AS FacultyName, d.name AS DepartmentName
            FROM Courses c
            LEFT JOIN Faculties f ON c.FacultyId = f.faculty_id
            LEFT JOIN Departments d ON c.DepartmentId = d.department_id
            ORDER BY c.name";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                var courses = new List<CourseDisplayItem>();
                while (reader.Read())
                {
                    courses.Add(new CourseDisplayItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        StudentsCount = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                        Faculty = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Department = reader.IsDBNull(4) ? "" : reader.GetString(4)
                    });
                }

                CoursesDataGrid.ItemsSource = courses;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка при завантаженні курсів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Клас для відображення у DataGrid
        public class CourseDisplayItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public int StudentsCount { get; set; }
            public string Faculty { get; set; } = "";
            public string Department { get; set; } = "";
        }

        private void AddCourse_Click(object sender, RoutedEventArgs e)
        {
            AddCourseWindow addCourseWindow = new AddCourseWindow();
            addCourseWindow.ShowDialog(); // Додає курс у базу
            LoadCoursesFromDatabase();    // Оновлюємо DataGrid
        }


        private void EditCourse_Click(object sender, RoutedEventArgs e)
        {
            if (CoursesDataGrid.SelectedItem is not CourseView selected)
            {
                MessageBox.Show("Оберіть курс");
                return;
            }

            // Відкриваємо вікно редагування
            EditCourseWindow w = new EditCourseWindow(
                selected.CourseNumber,
                selected.Faculty,
                selected.Department,
                selected.StudentCount
            )
            {
                Owner = this
            };

            if (w.ShowDialog() == true)
            {
                try
                {
                    using SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();

                    // Отримуємо department_id
                    int deptId = GetDepartmentIdByName(w.Department);
                    if (deptId == 0)
                    {
                        MessageBox.Show($"Департамент '{w.Department}' не знайдено.");
                        return;
                    }

                    string updateQuery = @"
UPDATE Courses
SET name = @name, department_id = @deptId
WHERE course_id = @id";

                    using SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@name", w.CourseName);
                    cmd.Parameters.AddWithValue("@deptId", deptId);
                    // обов'язково CourseId

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("Курс не знайдено або дані не змінилися.");
                    }
                    else
                    {
                        // Перезавантажуємо DataGrid
                        LoadCourses();
                        MessageBox.Show("Курс успішно оновлено.");
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Помилка оновлення курсу: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
        }

        private int GetDepartmentIdByName(string departmentName)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = "SELECT department_id FROM Departments WHERE name = @name";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", departmentName);

            object result = cmd.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out int deptId))
                return deptId;

            return 0; // або кинути помилку
        }





        private void DeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            if (CoursesDataGrid.SelectedItem is not CourseView selected)
            {
                MessageBox.Show("Будь ласка, виділіть курс для видалення.");
                return;
            }

            var result = MessageBox.Show(
                $"Видалити курс '{selected.CourseNumber}' разом з усіма студентами та записами у розкладі?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1️⃣ Видаляємо записи з Enrollments
                    string deleteEnrollments = @"
DELETE FROM Enrollments
WHERE student_id IN (
    SELECT student_id FROM Students WHERE [Group] = @groupNumber
)";
                    using (var cmd = new SqlCommand(deleteEnrollments, conn))
                    {
                        cmd.Parameters.AddWithValue("@groupNumber", selected.CourseNumber);
                        cmd.ExecuteNonQuery();
                    }

                    // 2️⃣ Видаляємо студентів
                    string deleteStudents = "DELETE FROM Students WHERE [Group] = @groupNumber";
                    using (var cmd = new SqlCommand(deleteStudents, conn))
                    {
                        cmd.Parameters.AddWithValue("@groupNumber", selected.CourseNumber);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 3️⃣ Оновлюємо DataGrid
                LoadCourses();

                MessageBox.Show($"Курс '{selected.CourseNumber}' видалено!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Помилка при видаленні курсу: " + ex.Message);
            }
        }









        // ===== MAX/MIN BUTTONS =====
        private void MaxDepartments_Click(object sender, RoutedEventArgs e)
        {
            if (FacultiesDataGrid.ItemsSource is List<FacultyDepartmentItem> list)
            {
                // Групуємо по факультету
                var grouped = list
                    .GroupBy(f => f.Faculty)
                    .Select(g => new
                    {
                        Faculty = g.Key,
                        DepartmentCount = g.Count()
                    })
                    .OrderByDescending(g => g.DepartmentCount)
                    .ToList();

                // Створюємо список для DataGrid (показуємо всі департаменти факультету, але сортуємо за макс. спец.)
                var sortedList = new List<FacultyDepartmentItem>();
                foreach (var g in grouped)
                {
                    sortedList.AddRange(list.Where(f => f.Faculty == g.Faculty));
                }

                FacultiesDataGrid.ItemsSource = sortedList;
            }
        }

        private void MinDepartments_Click(object sender, RoutedEventArgs e)
        {
            if (FacultiesDataGrid.ItemsSource is List<FacultyDepartmentItem> list)
            {
                var grouped = list
                    .GroupBy(f => f.Faculty)
                    .Select(g => new { Faculty = g.Key, DepartmentCount = g.Count() })
                    .OrderBy(g => g.DepartmentCount)
                    .ToList();

                var sortedList = new List<FacultyDepartmentItem>();
                foreach (var g in grouped)
                    sortedList.AddRange(list.Where(f => f.Faculty == g.Faculty));

                FacultiesDataGrid.ItemsSource = sortedList;
            }
        }

        private void MaxStudents_Click(object sender, RoutedEventArgs e)
        {
            // Перетворюємо ItemsSource у список
            if (FacultiesDataGrid.ItemsSource is List<FacultyDepartmentItem> list)
            {
                // Сортуємо за StudentCount у порядку спадання
                var sorted = list.OrderByDescending(f => f.StudentCount).ToList();

                // Підставляємо відсортований список у DataGrid
                FacultiesDataGrid.ItemsSource = sorted;
            }
        }

        private void MinStudents_Click(object sender, RoutedEventArgs e)
        {
            if (FacultiesDataGrid.ItemsSource is List<FacultyDepartmentItem> list)
            {
                var sorted = list.OrderBy(f => f.StudentCount).ToList();
                FacultiesDataGrid.ItemsSource = sorted;
            }
        }
        private void LoadStudentsFromDatabase()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = "SELECT student_id, first_name, last_name, faculty, department, [group] FROM Students";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                var students = new List<Student>();
                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Faculty = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Department = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        Group = reader.IsDBNull(5) ? "" : reader.GetString(5)
                    });
                }

                StudentsDataGrid.ItemsSource = students;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка завантаження студентів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== STUDENTS COURSES =====
        private void AddStudentToCourse_Click(object sender, RoutedEventArgs e)
        {
            AddStudentToCourseWindow window = new AddStudentToCourseWindow();
            window.ShowDialog();

            // Після закриття можна оновити DataGrid курсів або студентів, якщо потрібно
            LoadCoursesFromDatabase();
            LoadStudentsFromDatabase();
        }


        private void RemoveStudentFromCourse_Click(object sender, RoutedEventArgs e)
        {
            RemoveStudentFromCourseWindow w = new()
            {
                Owner = this
            };
            w.ShowDialog();
        }



        private IEnumerable<string> GetGroups()
        {
            var groups = new List<string>();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                using SqlCommand cmd = new SqlCommand("SELECT DISTINCT [Group] FROM Students WHERE [Group] IS NOT NULL ORDER BY [Group]", conn);
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    groups.Add(r.GetString(0));
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Помилка завантаження груп: " + ex.Message);
            }
            return groups;
        }

        private void AddLesson_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddLessonWindow(GetGroups())
            {
                Owner = this
            };

            if (addWindow.ShowDialog() == true)
            {
                string group = addWindow.Group;
                string day = addWindow.Day;
                string subject = addWindow.Subject;
                string room = addWindow.Room;

                // Конвертуємо строки часу у TimeSpan
                if (!TimeSpan.TryParse(addWindow.StartTime, out TimeSpan startTime) ||
                    !TimeSpan.TryParse(addWindow.EndTime, out TimeSpan endTime))
                {
                    MessageBox.Show("Неправильний формат часу! Використовуйте hh:mm.");
                    return;
                }

                try
                {
                    using SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();

                    string query = @"
INSERT INTO Schedule (DayOfWeek, StartTime, EndTime, Course, Classroom, [Group])
VALUES (@day, @start, @end, @subject, @room, @group)";

                    using SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.Parameters.AddWithValue("@start", startTime); // TimeSpan
                    cmd.Parameters.AddWithValue("@end", endTime);     // TimeSpan
                    cmd.Parameters.AddWithValue("@subject", subject);
                    cmd.Parameters.AddWithValue("@room", room);
                    cmd.Parameters.AddWithValue("@group", group);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Пару додано!");
                    LoadSchedule();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Помилка БД: " + ex.Message);
                }
            }
        }



        private void EditLesson_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleDataGrid.SelectedItem is ScheduleView selected)
            {
                var groups = GetGroups(); // метод повертає список груп

                // Розділяємо час на початок та кінець
                string startTime = "", endTime = "";
                var times = selected.Time.Split('-');
                if (times.Length == 2)
                {
                    startTime = times[0].Trim();
                    endTime = times[1].Trim();
                }

                // Відкриваємо вікно редагування
                EditLessonWindow editWindow = new EditLessonWindow(
                    groups,
                    selected.Group,
                    selected.Day,
                    startTime,
                    endTime,
                    selected.Subject,
                    selected.Room
                )
                { Owner = this };

                if (editWindow.ShowDialog() == true)
                {
                    // Оновлюємо запис у БД
                    try
                    {
                        using SqlConnection conn = new SqlConnection(connectionString);
                        conn.Open();

                        string query = @"
UPDATE Schedule
SET 
    [Group] = @newGroup,
    DayOfWeek = @newDay,
    StartTime = @newStart,
    EndTime = @newEnd,
    Course = @newCourse,
    Classroom = @newRoom
WHERE 
    [Group] = @oldGroup AND
    DayOfWeek = @oldDay AND
    StartTime = @oldStart AND
    EndTime = @oldEnd AND
    Course = @oldCourse AND
    Classroom = @oldRoom";

                        using SqlCommand cmd = new SqlCommand(query, conn);
                        // Нові значення
                        cmd.Parameters.AddWithValue("@newGroup", editWindow.Group);
                        cmd.Parameters.AddWithValue("@newDay", editWindow.Day);
                        cmd.Parameters.AddWithValue("@newStart", TimeSpan.Parse(editWindow.StartTime));
                        cmd.Parameters.AddWithValue("@newEnd", TimeSpan.Parse(editWindow.EndTime));
                        cmd.Parameters.AddWithValue("@newCourse", editWindow.Subject);
                        cmd.Parameters.AddWithValue("@newRoom", editWindow.Room);

                        // Старі значення
                        cmd.Parameters.AddWithValue("@oldGroup", selected.Group);
                        cmd.Parameters.AddWithValue("@oldDay", selected.Day);
                        cmd.Parameters.AddWithValue("@oldStart", TimeSpan.Parse(startTime));
                        cmd.Parameters.AddWithValue("@oldEnd", TimeSpan.Parse(endTime));
                        cmd.Parameters.AddWithValue("@oldCourse", selected.Subject);
                        cmd.Parameters.AddWithValue("@oldRoom", selected.Room);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            MessageBox.Show("Пару оновлено!");
                        else
                            MessageBox.Show("Не вдалося знайти пару для оновлення.");

                        LoadSchedule();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Помилка БД при редагуванні: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Оберіть пару для редагування!");
            }
        }



        private void DeleteLesson_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleDataGrid.SelectedItem is ScheduleView selected)
            {
                var result = MessageBox.Show($"Ви дійсно хочете видалити пару: {selected.Subject}?",
                                             "Підтвердження",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    using SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();

                    string query = @"
DELETE FROM Schedule
WHERE [Group] = @group
  AND DayOfWeek = @day
  AND StartTime = @start
  AND EndTime = @end
  AND Course = @course
  AND Classroom = @classroom";

                    using SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@group", selected.Group);
                    cmd.Parameters.AddWithValue("@day", selected.Day);

                    // Розділяємо час
                    var times = selected.Time.Split('-');
                    if (times.Length == 2)
                    {
                        if (TimeSpan.TryParse(times[0].Trim(), out TimeSpan startTime))
                            cmd.Parameters.AddWithValue("@start", startTime);
                        else
                            cmd.Parameters.AddWithValue("@start", DBNull.Value);

                        if (TimeSpan.TryParse(times[1].Trim(), out TimeSpan endTime))
                            cmd.Parameters.AddWithValue("@end", endTime);
                        else
                            cmd.Parameters.AddWithValue("@end", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@course", selected.Subject);
                    cmd.Parameters.AddWithValue("@classroom", selected.Room);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        MessageBox.Show("Пару успішно видалено!");
                    else
                        MessageBox.Show("Не вдалося знайти пару для видалення.");

                    LoadSchedule();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Помилка БД при видаленні: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Оберіть пару для видалення!");
            }
        }



        private void StudentFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyStudentFilter();
        }

       

        private void ApplyStudentFilter()
        {
            var filtered = allStudents.AsEnumerable();

            if (FilterFacultyComboBox.SelectedItem is FacultyItem f && f.Id != 0)
                filtered = filtered.Where(s => s.Faculty == f.Name);

            if (FilterGroupComboBox.SelectedItem is string g && g != "Усі групи")
                filtered = filtered.Where(s => s.Group == g);

            if (FilterStudyFormComboBox.SelectedItem is string sf && sf != "Усі форми")
                filtered = filtered.Where(s => s.StudyForm == sf);

            if (FilterFinanceComboBox.SelectedItem is string fund && fund != "Усі фінансування")
                filtered = filtered.Where(s => s.Funding == fund);

            StudentsDataGrid.ItemsSource = filtered.ToList();
        }


       

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSchedule();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
