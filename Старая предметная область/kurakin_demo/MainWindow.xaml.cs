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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace kurakin_demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = Login.Text.Trim();
            string password = Password.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин или пароль!");
                return;
            }
            using (var context = new Kurakin_ISIP43Entities())
            {
                var user = await context.users.
                    Where(u => u.username == username).
                    FirstOrDefaultAsync();
                if (user == null)
                {
                    MessageBox.Show("Неправильный логин или пароль,", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (user.islocked.HasValue && user.islocked.Value)
                {
                    MessageBox.Show("Вы заблокированы,пожалуйста,обратитесь к админисратору", "Доступ заблокирован", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (user.lastLoginDate.HasValue && (DateTime.Now - user.lastLoginDate.Value).TotalDays > 30 && user.role != "Admin")
                {
                    user.islocked = true;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Ваша учетная запись заблокирована из-за длительноо отсуствия", "Доступ заблокирован", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (user.password == password)
                {
                    user.lastLoginDate = DateTime.Now;
                    await context.SaveChangesAsync();
                    MessageBox.Show($"Добро пожаловать {username}", "Доступ разрешен", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (user.isFirstLogin.HasValue && user.isFirstLogin.Value)
                    {
                        ChangePassword changePasswordWindow = new ChangePassword(user.id);
                        changePasswordWindow.ShowDialog();
                    }
                    else
                    {
                        if (user.role == "Admin")
                        {
                            AdminWindow adminWindow = new AdminWindow();
                            adminWindow.ShowDialog();
                        }
                        else
                        {
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.ShowDialog();

                        }
                        this.Close();
                    }
                }
                else { 
                    user.failedLoginAttempts++;
                    if (user.failedLoginAttempts == 3)
                    {
                        user.islocked = true;
                        MessageBox.Show("Вы заблокированы после неудачаных 3 попыток", "Доступ запрещен",MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else {
                        int attemptsLeft = 3 - (user.failedLoginAttempts ?? 0);
                        MessageBox.Show($"Неправильный логин или пароль.Осталось попыток: {attemptsLeft}.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);

                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
