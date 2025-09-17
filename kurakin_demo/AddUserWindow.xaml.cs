using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace kurakin_demo
{
    /// <summary>
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        internal users NewUser { get; private set; }
        public AddUserWindow()
        {
            InitializeComponent();
            NewUser = null;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string userName = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string role = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(userName) 
                || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.","Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var newUser = new users
            {
                firstname = firstName,
                lastname = lastName,
                username = userName,
                password = password,
                role = role,
                isFirstLogin = true,
                islocked = false,
                failedLoginAttempts = 0,

            };
            NewUser = newUser;
            this.DialogResult = true;
            this.Close();
        }
    }
}
