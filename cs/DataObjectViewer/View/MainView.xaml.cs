namespace DataObjectViewer.View
{
	using System.Windows;
	using DataObjectViewer.ViewModel;

	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : Window
	{
		public MainView()
		{
			this.InitializeComponent();
		}

		public MainViewModel ViewModel
		{
			get { return this.DataContext as MainViewModel; }
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			e.Effects = DragDropEffects.Copy;
			e.Handled = true;
			base.OnDragEnter(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			e.Effects = DragDropEffects.Copy;
			e.Handled = true;
			base.OnDragOver(e);
		}

		protected override void OnDrop(DragEventArgs e)
		{
			if (this.ViewModel.AddDataObjectCommand.CanExecute(e.Data))
				this.ViewModel.AddDataObjectCommand.Execute(e.Data);

			e.Effects = DragDropEffects.Copy;
			e.Handled = true;
			base.OnDrop(e);
		}
	}
}
