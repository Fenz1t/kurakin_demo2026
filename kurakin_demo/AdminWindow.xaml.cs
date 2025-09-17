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
using System.Data.Entity;

namespace kurakin_demo
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadingUsers();
        }
        private async void LoadingUsers()
        {
            using (var context = new Kurakin_ISIP43Entities())
            {
                var users = await context.users.ToListAsync();
                UsersGrid.ItemsSource = users;
            }
        }
        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var newUserWindow = new AddUserWindow();
            if (newUserWindow.ShowDialog() == true && newUserWindow.NewUser != null)
            {
                var newUser = newUserWindow.NewUser;
                using (var context = new Kurakin_ISIP43Entities())
                {
                    if (await context.users.AnyAsync(u => u.username == newUser.username))
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        try
                        {
                            context.users.Add(newUser);
                            await context.SaveChangesAsync();
                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении данных {ex}", "Ошибка");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Добавление отменено");
            }
        }

        private async void Unblock_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is users selectedUser)
            {
                using (var context = new Kurakin_ISIP43Entities())
                {

                    var user = await context.users.FindAsync(selectedUser.id);

                    if (user != null)
                    {
                        user.islocked = false;
                        user.lastLoginDate = null; 

                        await context.SaveChangesAsync();

                        LoadingUsers();

                        MessageBox.Show("Пользователь разблокирован", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден в базе данных", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для разблокировки", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new Kurakin_ISIP43Entities())
            {
                foreach (var user in UsersGrid.ItemsSource as IEnumerable<users>)
                {
                    var existingUser = await context.users.FindAsync(user.id);
                    if (existingUser != null)
                    {
                        existingUser.lastname = user.lastname;
                        existingUser.firstname = user.firstname;
                        existingUser.role = user.role;
                        existingUser.username = user.username;
                        existingUser.islocked = user.islocked;
                    }
                }
                await context.SaveChangesAsync();
                LoadingUsers();
                MessageBox.Show("Изменения успешно сохранены","Успех",MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }
    }
}
