using System.ComponentModel.Composition;
using System.Windows;
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
		}

		[Import]
		public DeviceListViewModel ViewModel
		{
			get { return (DeviceListViewModel)this.DataContext; }
			private set { this.DataContext = value; }
		}
	}
}
