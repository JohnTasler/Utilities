namespace DataObjectViewer.ViewModel
{
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Input;
	using DataObjectViewer.ComponentModel.Mvvm;

	public class MainViewModel : ObservableObject
	{
		#region Instance Fields
		private ObservableCollection<DataObjectViewModel> _dataObjectsList;
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
				new DataObjectViewModel(this, Clipboard.GetDataObject()) { Name = "Clipboard" }
			};
			this.dataObjects = new ListCollectionView(_dataObjectsList);
		}
		#endregion Constructors

		#region Properties
		public ApplicationState ApplicationState
		{
			get { return this.applicationState; }
			set { this.SetProperty(ref this.applicationState, value, "ApplicationState"); }
		}
		private ApplicationState applicationState;

		public ListCollectionView DataObjects
		{
			get { return this.dataObjects; }
		}
		private ListCollectionView dataObjects;
		#endregion Properties

		#region AddDataObjectCommand

		/// <summary>
		/// Gets the AddDataObject command.
		/// </summary>
		public ICommand AddDataObjectCommand
		{
			get
			{
				return this.addDataObjectCommand ??
					(this.addDataObjectCommand = new RelayCommand<DataObject>(
						this.AddDataObjectCommandExecute, this.AddDataObjectCommandCanExecute));
			}
		}
		private RelayCommand<DataObject> addDataObjectCommand;

		private bool AddDataObjectCommandCanExecute(DataObject dataObject)
		{
			return dataObject != null;
		}

		private void AddDataObjectCommandExecute(DataObject dataObject)
		{
			var index = _dataObjectsList.Count + 1;
			var nameBase = "Dropped Data Object";
			string name;
			do { name = nameBase + " " + index++; }
			while (_dataObjectsList.Any(d => d.Name.Equals(name)));

			var vm = new DataObjectViewModel(this, dataObject) { Name = name };
			_dataObjectsList.Add(vm);
		}

		#endregion AddDataObjectCommand

	}
}
