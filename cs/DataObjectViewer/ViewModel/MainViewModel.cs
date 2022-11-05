namespace DataObjectViewer.ViewModel
{
	using System;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Input;
	using DataObjectViewer.ComponentModel;
	using DataObjectViewer.Properties;
	using DataObjectViewer.View;
	using Tasler.ComponentModel;
	using Tasler.Interop;
	using Tasler.Interop.User;

	public class MainViewModel : INotifyPropertyChanged
	{
		#region Instance Fields
		private readonly ObservableCollection<DataObjectViewModel> _dataObjectsList;
		#endregion Instance Fields

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MainViewModel"/> class.
		/// </summary>
		public MainViewModel()
		{
			// Create the list of data objects and collection view around it
			_dataObjectsList = new ObservableCollection<DataObjectViewModel>()
			{
				new DataObjectViewModel(this, Clipboard.GetDataObject()) { Name = Strings.ClipboardDisplayName }
			};
			_dataObjects = new ListCollectionView(_dataObjectsList);

			// Subscribe to data change notifications from the Clipboard
			WindowMessageBroadcastProcessor.Instance[WM.CLIPBOARDUPDATE] +=
				(s, e) =>
				{
					App.Current.Dispatcher.BeginInvoke(() =>
					{
						Debug.WriteLine("MainViewModel.WM_CLIPBOARDUPDATE:");
						_dataObjectsList[0] = new DataObjectViewModel(this, Clipboard.GetDataObject()) { Name = Strings.ClipboardDisplayName };
					});
				};
			UserApi.AddClipboardFormatListener(WindowMessageBroadcastProcessor.Instance.WindowHandle);
		}
		#endregion Constructors

		#region Properties

		public ApplicationState ApplicationState
		{
			get { return _applicationState; }
			set { this.PropertyChanged.SetProperty(this, value, ref _applicationState); }
		}
		private ApplicationState _applicationState;

		public ListCollectionView DataObjects
		{
			get { return _dataObjects; }
		}
		private ListCollectionView _dataObjects;
		#endregion Properties

		#region AddDataObjectCommand

		/// <summary>
		/// Gets the AddDataObject command.
		/// </summary>
		public ICommand AddDataObjectCommand
		{
			get
			{
				return _addDataObjectCommand ??
					(_addDataObjectCommand = new RelayCommand<DataObject>(
						this.AddDataObjectCommandExecute, this.AddDataObjectCommandCanExecute));
			}
		}
		private RelayCommand<DataObject> _addDataObjectCommand;

		private bool AddDataObjectCommandCanExecute(DataObject dataObject)
		{
			return dataObject != null;
		}

		private void AddDataObjectCommandExecute(DataObject dataObject)
		{
			var index = _dataObjectsList.Count + 1;
			var nameBase = Strings.DroppedDataObjectBaseDisplayName;
			string name;
			do { name = nameBase + " " + index++; }
			while (_dataObjectsList.Any(d => d.Name.Equals(name)));

			var vm = new DataObjectViewModel(this, dataObject) { Name = name };
			_dataObjectsList.Add(vm);
		}

		#endregion AddDataObjectCommand

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion INotifyPropertyChanged
	}
}
