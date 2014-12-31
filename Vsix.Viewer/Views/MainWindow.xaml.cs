using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Vsix.Viewer.Helpers;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            SetLanguageMenuSelection();

            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
        }

        private void SetLanguageMenuSelection()
        {
            var langs = ResourcesHelper.InstalledCultures();
            if (langs.Count < 2)
            {
                // only local culture is present...
                SeparatorLanguagesBefore.Visibility = Visibility.Collapsed;
                SeparatorLanguagesAfter.Visibility = Visibility.Collapsed;
                Languages.Visibility = Visibility.Collapsed;
                return;
            }

            SeparatorLanguagesBefore.Visibility = Visibility.Visible;
            SeparatorLanguagesAfter.Visibility = Visibility.Visible;
            Languages.Visibility = Visibility.Visible;
            Languages.Header = ResourcesHelper.Instance.GetString("menuSetLanguage"); // english only


            // var langs = new[] {"en-US","de-De", "ja-JP"};
            foreach (string slang in langs)
            {
                string display = CultureInfo.GetCultureInfo(slang).DisplayName;
                var newLanguage = new MenuItem { Header = display, Name = slang.Replace('-', '_') };
                newLanguage.Click += ChangeLanguage;
                if (slang.Equals("en-US"))
                {
                    newLanguage.IsChecked = true;
                    Languages.Header = string.Format("{0} [{1}]", ResourcesHelper.Instance.GetString("menuSetLanguage"), display);
                }
                Languages.Items.Add(newLanguage);
            }

        }
        private void ChangeLanguage(object sender, RoutedEventArgs args)
        {
            var menuItem = args.Source as MenuItem;
            if (menuItem==null) return;

            foreach (MenuItem m in Languages.Items)
                m.IsChecked = false;

            var newLanguage = menuItem.Name.Replace('_', '-');
            ResourcesHelper.Instance.SetAppCulture(newLanguage);

            _viewModel.InitResources();  

            menuItem.IsChecked = true;
            Languages.Header = string.Format("{0} [{1}]", ResourcesHelper.Instance.GetString("menuSetLanguage"), ResourcesHelper.Instance.DisplayName);
        }

        public void ExitApplication(object sender, RoutedEventArgs e)
        {
            Window window = GetWindow(this);
            if (window != null)
                window.Close();
        }        
    }
}
