using RawInputExplorer.Views;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;

namespace RawInputExplorer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var useOldWindow = true;
			if (!useOldWindow)
			{
				this.MainWindow = new MainView();

				var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
				var container = new CompositionContainer(catalog);
				container.ComposeParts(this.MainWindow);
			}
			else
			{
				this.MainWindow = new MainWindow();
			}

			this.MainWindow.Show();
		}
	}
}
