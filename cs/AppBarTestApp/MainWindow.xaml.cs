using AppBarTestApp.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppBarTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new MainWindowViewModel();
            this.InitializeComponent();
        }

        MainWindowViewModel ViewModel => (MainWindowViewModel)this.DataContext;

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (PresentationSource.FromVisual(this) is HwndSource source)
            {
                this.ViewModel.RegisterAppBar(source);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.ViewModel.UnregisterAppBar();
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.IsPinned = !this.ViewModel.IsPinned;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }
    }
}
