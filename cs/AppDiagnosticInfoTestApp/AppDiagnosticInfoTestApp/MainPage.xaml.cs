using Windows.UI.Xaml.Controls;

namespace AppDiagnosticInfoTestApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public MainPageViewModel ViewModel { get; } = new MainPageViewModel(ViewModelType.RunningApps);
    }
}
