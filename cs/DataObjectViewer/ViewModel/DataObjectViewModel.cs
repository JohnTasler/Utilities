﻿namespace DataObjectViewer.ViewModel
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Runtime.InteropServices.ComTypes;
	using Tasler.ComponentModel;
	using Tasler.Interop.Com;

	public class DataObjectViewModel : Child<MainViewModel>, INotifyPropertyChanged, IModelContainer<System.Windows.IDataObject>
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

		public IDataObject ComModel => this.Model as IDataObject;

		public string Name
		{
			get { return this.name; }
			set { this.PropertyChanged.SetProperty(this, value, ref this.name); }
		}
		private string name;

		public event PropertyChangedEventHandler PropertyChanged;

		public IEnumerable<FormatViewModel> Formats
		{
			get { return this.GetFormats(); }
		}

		#endregion Properties

		#region Private Implementation
		private IEnumerable<FormatViewModel> GetFormats()
		{
			var dataObject = this.Model as IDataObject;
			var enumFormatEtc = dataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
			return enumFormatEtc.AsEnumerable<IEnumFORMATETC, FORMATETC>(ComEnumExtensions.FetchFORMATETC)
				.Select(f => new FormatViewModel(this, f));
		}
		#endregion Private Implementation
	}
}
