namespace DataObjectViewer.ViewModel
{
	using System;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Input;
	using Tasler.ComponentModel;
	using DataObjectViewer.ComponentModel;
	using DataObjectViewer.Properties;
	using DataObjectViewer.View;
	using System.ComponentModel;

	public class MainViewModel : INotifyPropertyChanged, IAdviseSink, IDisposable
	{
		#region Instance Fields
		private ObservableCollection<DataObjectViewModel> _dataObjectsList;
		private int _adviseCookie;
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
			EventManager.RegisterClassHandler(typeof(MainView), DataObject.SettingDataEvent,
				new RoutedEventHandler((s, e) => { Debug.WriteLine($"MainViewModel.SettingDataEvent: {e.RoutedEvent.Name}"); }), true);

			// Subscribe to data change notifications from the Clipboard
			FORMATETC formatEtc = new FORMATETC
			{
				cfFormat = 0,
				ptd = IntPtr.Zero,
				dwAspect = (DVASPECT)(-1),
				lindex = -1,
				tymed = (TYMED)(-1)
			};
			_dataObjectsList[0].ComModel.DAdvise(ref formatEtc, ADVF.ADVF_NODATA, this, out _adviseCookie);
		}
		#endregion Constructors

		#region Destruction and Disposal
		~MainViewModel()
		{
			Dispose();
			GC.SuppressFinalize(this);
		}

		public void Dispose()
		{
			if (_adviseCookie != 0)
			{
				_dataObjectsList[0].ComModel.DUnadvise(_adviseCookie);
				_adviseCookie = 0;
			}
		}

		#endregion Destruction and Disposal

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

		#region IAdviseSink

		public void OnDataChange(ref FORMATETC format, ref STGMEDIUM stgmedium)
		{
			var formatViewModel = new FormatViewModel(_dataObjectsList[0], format);
			Debug.WriteLine("MainViewModel.OnDataChange: " +
				$"format={formatViewModel.FormatName},{formatViewModel.MediumType},{formatViewModel.TargetDevice} stgmedium.tymed={stgmedium.tymed}");
		}

		public void OnViewChange(int aspect, int index)
		{
			Debug.WriteLine($"MainViewModel.OnViewChange: aspect={aspect} index={index}");
		}

		public void OnRename(IMoniker moniker)
		{
			Debug.WriteLine($"MainViewModel.OnRename:");
		}

		public void OnSave()
		{
			Debug.WriteLine($"MainViewModel.OnSave:");
		}

		public void OnClose()
		{
			Debug.WriteLine($"MainViewModel.OnClose:");
		}

		#endregion IAdviseSink

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AddClipboardFormatListener(IntPtr hwnd);
	}
}
