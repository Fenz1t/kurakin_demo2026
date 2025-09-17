using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private readonly int _userId;
        public ChangePassword(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = CurrentPassword.Password;
            string newPassword = NewPassword.Password;
            string confirmPassword = ConfirmPassword.Password;

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Все поля обязательны к заполнению", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (newPassword != ConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var context = new Kurakin_ISIP43Entities())
                {
                    var user = await context.users.Where(u => u.id == _userId).FirstOrDefaultAsync();

                    if (user == null)
                    {

                        MessageBox.Show("Пользователь не найден","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if(user.password != currentPassword)
                    {
                        MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    user.password = newPassword;
                    user.isFirstLogin = false;

                    context.SaveChanges();
                    MessageBox.Show("Пароль успешно измененен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Ошибка при изменении пароля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
