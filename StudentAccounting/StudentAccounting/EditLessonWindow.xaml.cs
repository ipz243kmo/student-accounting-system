using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfMessageBox = System.Windows.MessageBox;

namespace StudentAccounting
{
    public partial class EditLessonWindow : Window
    {
        // Властивості для передачі назад у Admin
        public string Group { get; private set; } = string.Empty;
        public string Day { get; private set; } = string.Empty;
        public string StartTime { get; private set; } = string.Empty;
        public string EndTime { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string Room { get; private set; } = string.Empty;

        public EditLessonWindow(IEnumerable<string> groups, string group, string day, string startTime, string endTime, string subject, string room)
        {
            InitializeComponent();

            // Заповнюємо ComboBox групами
            GroupComboBox.ItemsSource = groups;
            GroupComboBox.SelectedItem = group;

            DayComboBox.SelectedItem = day;
            StartTimeTextBox.Text = startTime;
            EndTimeTextBox.Text = endTime;
            SubjectTextBox.Text = subject;
            RoomTextBox.Text = room;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
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

            // Отримуємо реальний текст з ComboBoxItem
            Group = (GroupComboBox.SelectedItem as string) ?? GroupComboBox.SelectedItem.ToString();
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
