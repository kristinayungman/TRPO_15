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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pr15_Shop.Pages
{
    /// <summary>
    /// Логика взаимодействия для Vhod.xaml
    /// </summary>
    public partial class Vhod : Page
    {
        private const string ManagerPassword = "1234";
        public Vhod()
        {
            InitializeComponent();
        }
        private void BtnUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы зашли за пользователя");
            NavigationService.Navigate(new MainPage());
        }

        private void BtnManager_Click(object sender, RoutedEventArgs e)
        {
            if (TextPass.Text == ManagerPassword)
            {
                MessageBox.Show("Вы зашли как менеджер");
                NavigationService.Navigate(new ManagerPage());
            }
            else
            {
                MessageBox.Show("Неверный пароль");
            }
        }
        ////Для Enter
        //private void ManagerPasswordBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        ManagerClick(sender, e);
        //    }
        //}
        //private void ManagerClick(object sender, RoutedEventArgs e)
        //{
        //    if (ManagerPasswordBox.Password == ManagerPassword)
        //    {
        //        ManagerDialog.IsOpen = false;
        //        MessageBox.Show("Вы зашли как менеджер");
        //        NavigationService.Navigate(new ManagerPage());
        //    }
        //    else
        //    {
        //        MessageBox.Show("Неверный пароль");
        //    }
        //}
        //private void Cansle_Dialoge(object sender, RoutedEventArgs e)
        //{
        //    ManagerDialog.IsOpen = false;
        //}
    }
}

