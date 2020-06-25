using System.Windows;
using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.Helpers;
using BO.Elastic.Panel.ViewModels;

namespace BO.Elastic.Panel
{
    /// <summary>
    ///     Interaction logic for SavePasswordWindow.xaml
    /// </summary>
    public partial class SavePasswordWindow : Window
    {
        private readonly SaveLoginDataWindowViewModel viewModel = new SaveLoginDataWindowViewModel();

        public SavePasswordWindow()
        {
            DataContext = viewModel;
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
            loginData.Password = tbPassword.Password;
            try
            {
                LoginDataHelper.SaveLoginData(loginData);
                SshLoginDataContainer.LoginData = LoginDataHelper.GetCachedLoginData();
                Close();
            }
            catch
            {
                MessageBox.Show("Błąd podczas zapisu danych. Sprawdź czy wypełniono login.");
            }
        }
    }
}