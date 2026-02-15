using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using WpfMessageBox = System.Windows.MessageBox;
namespace StudentAccounting
{
    public partial class AddLessonWindow : Window
    {
        // Властивості для передачі даних
        public string Group { get; private set; } = string.Empty;
        public string Day { get; private set; } = string.Empty;
        public string StartTime { get; private set; } = string.Empty;
        public string EndTime { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string Room { get; private set; } = string.Empty;

        public AddLessonWindow(IEnumerable<string> groups)
        {
            InitializeComponent();

            // Заповнюємо ComboBox групами
            GroupComboBox.ItemsSource = groups;

            // День за замовчуванням
            DayComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (GroupComboBox.SelectedItem == null ||
                DayComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(StartTimeTextBox.Text) ||
                string.IsNullOrWhiteSpace(EndTimeTextBox.Text) ||
                string.IsNullOrWhiteSpace(SubjectTextBox.Text) ||
                string.IsNullOrWhiteSpace(RoomTextBox.Text))
            {
                WpfMessageBox.Show("Будь ласка, заповніть всі поля!");
                return;
            }

            Group = GroupComboBox.SelectedItem.ToString();
            Day = (DayComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            StartTime = StartTimeTextBox.Text.Trim();
            EndTime = EndTimeTextBox.Text.Trim();
            Subject = SubjectTextBox.Text.Trim();
            Room = RoomTextBox.Text.Trim();

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
