namespace DataObjectViewer.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices.ComTypes;
	using DataObjectViewer.ComponentModel.Mvvm;
	using DataObjectViewer.Utility;

	public class DataObjectViewModel : ParentedObservableObject<MainViewModel>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DataObjectViewModel"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="model">The model.</param>
		public DataObjectViewModel(MainViewModel parent, System.Windows.IDataObject model)
			: base(parent)
		{
			this.Model = model;
		}
		#endregion Constructors

		#region Properties

		public System.Windows.IDataObject Model { get; private set; }

		public string Name
		{
			get { return this.name; }
			set { this.SetProperty(ref this.name, value, "Name"); }
		}
		private string name;

		public IEnumerable<FormatViewModel> Formats
		{
			get { return this.GetFormats(); }
		}
		//private IEnumerable<FormatViewModel> formats;

		#endregion Properties

		#region Private Implementation
		private IEnumerable<FormatViewModel> GetFormats()
		{
			var dataObject = this.Model as System.Runtime.InteropServices.ComTypes.IDataObject;
			var enumFormatEtc = dataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
			return enumFormatEtc.AsEnumerable(4).Select(f => new FormatViewModel(this, f));
		}
		#endregion Private Implementation
	}
}
