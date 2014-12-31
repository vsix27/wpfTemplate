using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfDemoWithMvvmp.ViewModels;

namespace WpfDemoWithMvvmp.Views
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
            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
        }

        public void ExitApplication(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
                window.Close();
        }        
    }
}
