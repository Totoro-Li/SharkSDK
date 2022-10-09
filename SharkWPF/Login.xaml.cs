using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using SharkSDK;

namespace SharkWPF
{
    public class Shared
    {
        public static BrowserHelper helper;
    }

    public partial class Login : Page
    {
        private LoginPageViewModel _loginPageViewModel = new();
        private SecureString ttShituPassword => TTShituPassword.SecurePassword;
        private SecureString iaaaPassword => IAAAPassword.SecurePassword;

        public Login()
        {
            InitializeComponent();
            DataContext = _loginPageViewModel;
        }

        private void DriverPickFile_OnClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            bool? result = fileDialog.ShowDialog();
            switch (result)
            {
                case true:
                    var file = fileDialog.FileName;
                    break;
                default:
                    break;
            }
        }

        private void IAAAUsernameRegex(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void Go_BtnClick(object sender, RoutedEventArgs e)
        {
            if (_loginPageViewModel.IAAAUsername != null)
            {
                Shared.helper.SetCredential(_loginPageViewModel.IAAAUsername, iaaaPassword);
                Shared.helper.IAAALogin();
                SharkNavigationService.Navigate(1);
            }
            else
                SharkNavigationService.SnackbarMessage("Empty credential field!");
        }
    }

    public class LoginPageViewModel : ViewModelBase
    {
        private BrowserBackend _selectedBackend = BrowserBackend.CHROME;
        private string? _ttShituUsername;
        private string? _ttShituPassword;
        private string? _iaaaUsername;
        private string? _iaaaPassword;

        public IEnumerable<BrowserBackend> BrowserBackends =>
            Enum.GetValues(typeof(BrowserBackend))
                .Cast<BrowserBackend>();

        public BrowserBackend SelectedBackend
        {
            get => _selectedBackend;
            set
            {
                _selectedBackend = value;
                OnPropertyChanged();
            }
        }

        public string? TTShituUsername
        {
            get => _ttShituUsername;
            set
            {
                _ttShituUsername = value;
                OnPropertyChanged();
            }
        }

        public string? IAAAUsername
        {
            get => _iaaaUsername;
            set
            {
                _iaaaUsername = value;
                OnPropertyChanged();
            }
        }
    }
}