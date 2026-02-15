using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

using MsgBox = System.Windows.MessageBox;
using MediaBrushes = System.Windows.Media.Brushes;

namespace StudentAccounting
{
    public partial class Register : Window
    {
        private readonly string connectionString =
            "Server=localhost;Database=University;Trusted_Connection=True;TrustServerCertificate=True;";

        private int StudentId;

        private List<string> allCourses = new List<string>();
        private List<StudentCourse> studentCourses = new List<StudentCourse>();
        private List<AvailableCourse> availableCourses = new List<AvailableCourse>();
        private List<ScheduleItem> schedule = new List<ScheduleItem>();
        private List<Payment> payments = new List<Payment>();

        #region Конструктори

        public Register() : this(5) { } // Конструктор за замовчуванням

        public Register(int studentId)
        {
            InitializeComponent();
            StudentId = studentId;

            LoadStudentInfo();
            LoadBuildings();
            LoadAllCourses();
            LoadStudentCourses();
            LoadSchedule();
            LoadPayments();
        }

        #endregion

        #region Моделі

        public class StudentCourse
        {
            public string CourseName { get; set; } = "";
            public int? Grade { get; set; }
        }

        public class AvailableCourse
        {
            public string CourseName { get; set; } = "";
        }

        public class ScheduleItem
        {
            public string Day { get; set; } = "";
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string Course { get; set; } = "";
            public string Classroom { get; set; } = "";
        }

        public class Payment
        {
            public int Id { get; set; }
            public string Amount { get; set; } = "";
            public string Status { get; set; } = "";
            public string Details { get; set; } = "";
            public DateTime Date { get; set; }
        }

        #endregion

        #region Інформація про студента

        private void LoadStudentInfo()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT first_name, last_name, faculty_id, department_id, courses, StudyForm,
                        CASE student_id
                            WHEN 5 THEN 'Computer Science'
                            WHEN 6 THEN 'Geology'
                            WHEN 7 THEN 'Computer Engineering'
                            WHEN 9 THEN 'Management'
                            WHEN 10 THEN 'Linguistics'
                            WHEN 12 THEN 'Defense Studies'
                            WHEN 13 THEN 'Law'
                            WHEN 14 THEN 'Linguistics'
                            WHEN 15 THEN 'Economics'
                            WHEN 16 THEN 'E-learning'
                            WHEN 18 THEN 'Geoinformatics'
                            WHEN 19 THEN 'Cybersecurity'
                            WHEN 20 THEN 'Cybersecurity'
                            WHEN 21 THEN 'Finance'
                            WHEN 22 THEN 'Media Studies'
                            WHEN 23 THEN 'Ecology'
                            WHEN 24 THEN 'Political Science'
                            WHEN 25 THEN 'Computer Science'
                            WHEN 26 THEN 'Media Studies'
                            WHEN 28 THEN 'Linguistics'
                            WHEN 29 THEN 'Computer Engineering'
                            ELSE ''
                        END AS Specialty,
                        CASE student_id
                            WHEN 5 THEN 'Bachelor CS'
                            WHEN 6 THEN 'Bachelor Geology'
                            WHEN 7 THEN 'Bachelor CE'
                            WHEN 9 THEN 'Bachelor Management'
                            WHEN 10 THEN 'Bachelor Linguistics'
                            WHEN 12 THEN 'Bachelor Defense'
                            WHEN 13 THEN 'Bachelor Law'
                            WHEN 14 THEN 'Bachelor Linguistics'
                            WHEN 15 THEN 'Bachelor Economics'
                            WHEN 16 THEN 'Bachelor E-learning'
                            WHEN 18 THEN 'Bachelor GIS'
                            WHEN 19 THEN 'Bachelor Cybersecurity'
                            WHEN 20 THEN 'Bachelor Cybersecurity'
                            WHEN 21 THEN 'Bachelor Finance'
                            WHEN 22 THEN 'Bachelor Media'
                            WHEN 23 THEN 'Bachelor Ecology'
                            WHEN 24 THEN 'Bachelor Politics'
                            WHEN 25 THEN 'Bachelor CS'
                            WHEN 26 THEN 'Bachelor Media'
                            WHEN 28 THEN 'Bachelor Linguistics'
                            WHEN 29 THEN 'Bachelor CE'
                            ELSE '—'
                        END AS Program,
                        CASE student_id
                            WHEN 5 THEN 'CS-1'
                            WHEN 6 THEN 'GEO-1'
                            WHEN 7 THEN 'CE-1'
                            WHEN 9 THEN 'MNG-1'
                            WHEN 10 THEN 'LIN-1'
                            WHEN 12 THEN 'DEF-1'
                            WHEN 13 THEN 'LAW-1'
                            WHEN 14 THEN 'LIN-2'
                            WHEN 15 THEN 'ECO-1'
                            WHEN 16 THEN 'ELE-1'
                            WHEN 18 THEN 'GIS-1'
                            WHEN 19 THEN 'CYB-1'
                            WHEN 20 THEN 'CYB-2'
                            WHEN 21 THEN 'FIN-1'
                            WHEN 22 THEN 'MED-1'
                            WHEN 23 THEN 'ECO-2'
                            WHEN 24 THEN 'POL-1'
                            WHEN 25 THEN 'CS-2'
                            WHEN 26 THEN 'MED-2'
                            WHEN 28 THEN 'LIN-3'
                            WHEN 29 THEN 'CE-2'
                            ELSE '—'
                        END AS [Group]
                    FROM Students
                    WHERE student_id=@StudentId
                ";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", StudentId);

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    FullNameTextBlock.Text = $"{reader["first_name"]?.ToString()} {reader["last_name"]?.ToString()}";
                    FacultyTextBlock.Text = reader["faculty_id"]?.ToString() ?? "";
                    DegreeTextBlock.Text = reader["department_id"]?.ToString() ?? "";
                    StudyFormTextBlock.Text = reader["StudyForm"]?.ToString() ?? "";
                    SpecialtyTextBlock.Text = reader["Specialty"]?.ToString() ?? "—";
                    ProgramTextBlock.Text = reader["Program"]?.ToString() ?? "—";
                    GroupTextBlock.Text = reader["Group"]?.ToString() ?? "—";
                }
            }
            catch (SqlException ex)
            {
                MsgBox.Show("Помилка завантаження інформації про студента: " + ex.Message,
                            "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Аудиторії

        private void LoadBuildings()
        {
            BuildingsComboBox.Items.Clear();
            BuildingsComboBox.Items.Add("Всі корпуси");
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT DISTINCT Building FROM Classrooms";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    BuildingsComboBox.Items.Add(reader.GetString(0));
            }
            catch (SqlException ex)
            {
                MsgBox.Show("Помилка бази (корпуси): " + ex.Message,
                            "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            BuildingsComboBox.SelectedIndex = 0;
            BuildingsComboBox.SelectionChanged += (s, e) => LoadClassrooms();
            LoadClassrooms();
        }

        private void LoadClassrooms()
        {
            string selectedBuilding = BuildingsComboBox.SelectedItem?.ToString();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT Name AS Аудиторія, Capacity AS Місць, Building AS Корпус FROM Classrooms";
                if (!string.IsNullOrEmpty(selectedBuilding) && selectedBuilding != "Всі корпуси")
                    query += " WHERE Building=@Building";

                using SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(selectedBuilding) && selectedBuilding != "Всі корпуси")
                    cmd.Parameters.AddWithValue("@Building", selectedBuilding);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                ClassroomsDataGrid.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MsgBox.Show("Помилка бази (аудиторії): " + ex.Message,
                            "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Курси

        private void LoadAllCourses()
        {
            allCourses.Clear();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT name FROM Courses";
                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    allCourses.Add(reader.GetString(0));
            }
            catch (SqlException ex)
            {
                MsgBox.Show("Помилка бази (всі курси): " + ex.Message,
                            "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStudentCourses()
        {
            studentCourses.Clear();
            availableCourses.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                // Курси студента
                string query = "SELECT courses FROM Students WHERE student_id=@StudentId";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", StudentId);
                object result = cmd.ExecuteScalar();
                List<string> studentCourseNames = new List<string>();
                if (result != DBNull.Value && result != null)
                    studentCourseNames = result.ToString()!.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()).ToList();

                // Оцінки студента
                Dictionary<string, int> gradesDict = new Dictionary<string, int>();
                string gradesQuery = @"
                    SELECT c.name, g.grade
                    FROM Grades_ g
                    INNER JOIN Courses c ON g.course_id=c.course_id
                    WHERE g.student_id=@StudentId";
                using SqlCommand cmdGrades = new SqlCommand(gradesQuery, conn);
                cmdGrades.Parameters.AddWithValue("@StudentId", StudentId);
                using SqlDataReader reader = cmdGrades.ExecuteReader();
                while (reader.Read())
                    gradesDict[reader.GetString(0)] = reader.GetInt32(1);

                foreach (var c in studentCourseNames)
                {
                    int? grade = gradesDict.ContainsKey(c) ? gradesDict[c] : null;
                    studentCourses.Add(new StudentCourse { CourseName = c, Grade = grade });
                }

                foreach (var c in allCourses)
                {
                    if (!studentCourseNames.Contains(c))
                        availableCourses.Add(new AvailableCourse { CourseName = c });
                }
            }
            catch (SqlException ex)
            {
                MsgBox.Show("Помилка бази (курси/оцінки студента): " + ex.Message,
                            "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            RefreshCoursesGrids();
        }

        private void RefreshCoursesGrids()
        {
            StudentCoursesDataGrid.ItemsSource = null;
            StudentCoursesDataGrid.ItemsSource = studentCourses;
            StudentCoursesDataGrid.LoadingRow += (s, e) =>
            {
                if (e.Row.Item is StudentCourse sc)
                    e.Row.Background = sc.Grade.HasValue ? MediaBrushes.LightGreen : MediaBrushes.White;
            };

            AvailableCoursesDataGrid.ItemsSource = null;
            AvailableCoursesDataGrid.ItemsSource = availableCourses;
        }

        private void EnrollButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableCoursesDataGrid.SelectedItem is not AvailableCourse course) return;

            if (studentCourses.Count >= 2)
            {
                MsgBox.Show("Ви вже записані на 2 курси!", "Запис", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool allPassed = studentCourses.All(sc => sc.Grade.HasValue && sc.Grade >= 60);
            if (!allPassed)
            {
                MsgBox.Show("Щоб записатися на новий курс, спочатку успішно складіть попередні!",
                            "Запис", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            studentCourses.Add(new StudentCourse { CourseName = course.CourseName, Grade = null });
            availableCourses.Remove(course);
            MsgBox.Show($"Курс '{course.CourseName}' зареєстровано успішно!", "Запис", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshCoursesGrids();
        }

        #endregion

        #region Розклад

        private void LoadSchedule()
        {
            schedule.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT DayOfWeek, StartTime, EndTime, Course, Classroom
                    FROM Schedule
                    WHERE StudentId=@StudentId";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", StudentId);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    schedule.Add(new ScheduleItem
                    {
                        Day = reader["DayOfWeek"]?.ToString() ?? "",
                        StartTime = (TimeSpan)reader["StartTime"],
                        EndTime = (TimeSpan)reader["EndTime"],
                        Course = reader["Course"]?.ToString() ?? "",
                        Classroom = reader["Classroom"]?.ToString() ?? ""
                    });
                }

                RefreshScheduleGrid();
            }
            catch (SqlException ex)
            {
                MsgBox.Show("Помилка завантаження розкладу: " + ex.Message,
                            "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DaysComboBox.SelectionChanged += (s, e) => RefreshScheduleGrid();
        }

        private void RefreshScheduleGrid()
        {
            if (DaysComboBox.SelectedItem == null)
            {
                ScheduleDataGrid.ItemsSource = schedule;
            }
            else
            {
                string selectedDay = (DaysComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                ScheduleDataGrid.ItemsSource = schedule.Where(s => s.Day == selectedDay).ToList();
            }
        }

        private void SortScheduleByTime_Click(object sender, RoutedEventArgs e)
        {
            ScheduleDataGrid.ItemsSource = schedule.OrderBy(s => s.StartTime).ToList();
        }

        private void ResetScheduleFilters_Click(object sender, RoutedEventArgs e)
        {
            DaysComboBox.SelectedIndex = -1;
            ScheduleDataGrid.ItemsSource = schedule;
        }

        #endregion

        #region Оплата

        private void LoadPayments()
        {
            payments.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT PaymentId, StudentId, AmountPerMonth, Status, PaymentDetails, PaymentDate
                    FROM Payments
                    WHERE StudentId=@StudentId
                    ORDER BY PaymentDate";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentId", StudentId);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    payments.Add(new Payment
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("PaymentId")),
                        Amount = reader["AmountPerMonth"]?.ToString() ?? "",
                        Status = reader["Status"]?.ToString() ?? "",
                        Details = reader["PaymentDetails"]?.ToString() ?? "",
                        Date = reader["PaymentDate"] != DBNull.Value ? (DateTime)reader["PaymentDate"] : DateTime.MinValue
                    });
                }

                PaymentsDataGrid.ItemsSource = null;
                PaymentsDataGrid.ItemsSource = payments;

                if (payments.Count == 0)
                    MsgBox.Show("Для цього студента записи про оплату відсутні.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MsgBox.Show("Помилка завантаження платежів: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
