using BO.Elastic.Panel.Command;
using BO.Elastic.Panel.ViewModels;
using BO.Elastic.Panel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BO.Elastic.BLL.Model;

namespace BO.Elastic.Panel
{
    /// <summary>
    /// Interaction logic for SavePasswordWindow.xaml
    /// </summary>
    public partial class SavePasswordWindow : Window
    {
        SaveLoginDataWindowViewModel viewModel = new SaveLoginDataWindowViewModel();

        public SavePasswordWindow()
        {
            this.DataContext = viewModel;
            Owner = Application.Current.MainWindow;
            InitializeComponent();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            tbLogin.Clear();
            tbPassword.Clear();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            LoginData loginData = new LoginData();
            loginData.Login = tbLogin.Text;
            loginData.Password = tbPassword.Password.ToString();
            LoginDataHelper.SaveLoginData(loginData);
            this.Close();
        }
    }
}
