using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using SharkSDK;
using SharkWPF;

namespace SharkWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public static class ZenNavigationService
    {
        public static readonly Dictionary<int, Page> Bindings = new();
        public static Page? Current;
        public static Frame Navframe { get; set; }

        public static void SetNavFrame(Frame navframe)
        {
            Navframe = navframe;
        }
        public static int AddPageBinding(int index, Page dest)
        {
            try
            {
                Bindings[index] = dest;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return 0;
        }

        public static Page GetPage(int index)
        {
            if (Bindings.ContainsKey(index))
                return Bindings[index];
            throw new ArgumentException("No desired page found or initialized");
        }

        public static void Navigate(int dest)
        {
            Navframe.NavigationService.Navigate(GetPage(dest));
            Current = GetPage(dest);
        }

        public static void Navigate(Page dest)
        {
            Navframe.NavigationService.Navigate(dest);
            Current = dest;
        }
    }

    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ConsoleHelper.InitConsoleSession();
            InitializeComponent();
            ZenNavigationService.SetNavFrame(Navframe);

        }
        private void MousedownDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            DragMove();
        }

        public void PageNavigateHandler(int dest)
        {
            try
            {
                ZenNavigationService.Navigate(dest);
            }
            catch (ArgumentException)
            {
                NavFrameSnackbar.MessageQueue?.Enqueue("You are not connected to Zenlite device!", null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        public void listViewItem_MouseOpenClick(object sender, MouseButtonEventArgs e)
        {
            //ListViewItem item = sender as ListViewItem;
            //object obj = item.Content;

            //navigate to a new page or open new window on user click
            Tg_Btn.IsChecked = false;
            var selectedItemText = SideBar.SelectedItem;
            PageNavigateHandler(SideBar.SelectedIndex);
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set tooltip visibility
            if (Tg_Btn.IsChecked == true)
            {
                TooltipLogin.Visibility = Visibility.Collapsed;
                TooltipSelect.Visibility = Visibility.Collapsed;
                TooltipConfig.Visibility = Visibility.Collapsed;
            }
            else
            {
                TooltipLogin.Visibility = Visibility.Visible;
                TooltipSelect.Visibility = Visibility.Visible;
                TooltipConfig.Visibility = Visibility.Visible;
            }
        }

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            Navframe.Opacity = 1;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            Navframe.Opacity = 0.6;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void HideBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.SingleBorderWindow; // Switch to single-bordered so Windows can take charge of the minimizing process
            WindowState = WindowState.Minimized;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => WindowStyle = WindowStyle.None));

        }
        private void NavFrame_OnNavigating(object sender, NavigatingCancelEventArgs e) {
            var ta = new ThicknessAnimation();
            ta.Duration = TimeSpan.FromSeconds(0.3);
            ta.DecelerationRatio = 0.7;
            ta.To = new Thickness(0 , 0 , 0 , 0);
            if (e.NavigationMode == NavigationMode.New) {         
                ta.From = new Thickness(500, 0, 0, 0);                                                  
            }
            else if (e.NavigationMode == NavigationMode.Back) {                
                ta.From = new Thickness(0 , 0 , 500 , 0);                                               
            }
            (e.Content as Page)?.BeginAnimation(MarginProperty , ta);
        }
        
        private void Navframe_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Navframe.Focus();
        }

        private void MaximizeBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void ConsoleToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            ConsoleHelper.ShowConsoleWindow();
        }

        private void ConsoleToggle_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ConsoleHelper.HideConsoleWindow();
        }
    }
}