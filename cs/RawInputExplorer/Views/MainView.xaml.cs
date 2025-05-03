using System.Windows;
using System.Windows.Input;
using RawInputExplorer.ViewModels;

namespace RawInputExplorer.Views
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : Window
	{
		public MainView()
		{
			this.InitializeComponent();

			this.Loaded += (s, e) => this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}

		public DeviceListViewModel ViewModel
		{
			get { return (DeviceListViewModel)this.DataContext; }
			private set { this.DataContext = value; }
		}
	}
}
