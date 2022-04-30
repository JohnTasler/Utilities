using AppDiagnosticInfoTestApp;
using Windows.UI.Xaml.Controls;

namespace ListAndLaunchApps
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public MainPageViewModel ViewModel { get; } = new MainPageViewModel(ViewModelType.InstalledApps);
    }
}
