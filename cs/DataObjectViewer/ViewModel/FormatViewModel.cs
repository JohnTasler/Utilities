namespace DataObjectViewer.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Text;
	using DataObjectViewer.ComponentModel.Mvvm;

	public class FormatViewModel : ParentedObservableObject<DataObjectViewModel>
	{
		public FormatViewModel(DataObjectViewModel parent, FORMATETC model)
			: base(parent)
		{
			this.Model = model;
		}

		public FORMATETC Model { get; private set; }

		public short Format
		{
			get { return this.Model.cfFormat; }
		}

		public string FormatName
		{
			get
			{
				if (_formatName == null)
				{
					var name = string.Empty;
					var value = (short)this.Model.cfFormat;
					if (0 <= value && value < (short)ClipboardFormat.PRIVATEFIRST && Enum.IsDefined(typeof(ClipboardFormat), value))
					{
						name = "CF_" + Enum.GetName(typeof(ClipboardFormat), value);
					}
					else if ((short)ClipboardFormat.PRIVATEFIRST <= value && value <= (short)ClipboardFormat.PRIVATELAST)
					{
						var offset = (short)(value - (short)ClipboardFormat.PRIVATEFIRST);
						name = "CF_" + ClipboardFormat.PRIVATEFIRST + " + 0x" + offset.ToString("X4");
					}
					else if ((short)ClipboardFormat.GDIOBJFIRST <= value && value <= (short)ClipboardFormat.GDIOBJLAST)
					{
						var offset = (short)(value - (short)ClipboardFormat.GDIOBJFIRST);
						name = "CF_" + ClipboardFormat.GDIOBJFIRST + " + 0x" + offset.ToString("X4");
					}
					else
					{
						try
						{
							name = "\"" + UserApi.GetClipboardFormatName(value) + "\"";
						}
						catch
						{
							name = "0x" + value.ToString("X4");
						}
					}

					_formatName = name;
				}

				return _formatName;
			}
		}
		private string _formatName;

		public string TargetDevice
		{
			get
			{
				if (_targetDevice == null)
				{
					var value = string.Empty;

					if (this.Model.ptd != IntPtr.Zero)
						value = "0x" + this.Model.ptd.ToString(IntPtr.Size == 4 ? "X8" : "X16");

					_targetDevice = value;
				}

				return _targetDevice;
			}
		}
		private string _targetDevice;

		public string Aspect
		{
			get
			{
				if (_aspect == null)
				{
					var value = this.Model.dwAspect;
					var names = new List<string>(4);

					if (value.HasFlag(DVASPECT.DVASPECT_CONTENT))
						names.Add("Content");
					if (value.HasFlag(DVASPECT.DVASPECT_THUMBNAIL))
						names.Add("Thumbnail");
					if (value.HasFlag(DVASPECT.DVASPECT_ICON))
						names.Add("Icon");
					if (value.HasFlag(DVASPECT.DVASPECT_DOCPRINT))
						names.Add("DocPrint");

					_aspect = string.Join(", ", names);
				}

				return _aspect;
			}
		}
		private string _aspect;

		public int? Index
		{
			get
			{
				if (this.Model.lindex < 0)
					return null;
				return this.Model.lindex;
			}
		}

		public string MediumType
		{
			get
			{
				if (_mediumType == null)
				{
					var value = this.Model.tymed;
					var names = new List<string>(8);

					if (value == TYMED.TYMED_NULL)
					{
						names.Add("null");
					}
					else
					{
						if (value.HasFlag(TYMED.TYMED_HGLOBAL))
							names.Add("HGLOBAL");
						if (value.HasFlag(TYMED.TYMED_FILE))
							names.Add("File");
						if (value.HasFlag(TYMED.TYMED_ISTREAM))
							names.Add("IStream");
						if (value.HasFlag(TYMED.TYMED_ISTORAGE))
							names.Add("IStorage");
						if (value.HasFlag(TYMED.TYMED_GDI))
							names.Add("GDI");
						if (value.HasFlag(TYMED.TYMED_MFPICT))
							names.Add("Metafile");
						if (value.HasFlag(TYMED.TYMED_ENHMF))
							names.Add("Enhanced Metafile");
					}

					_mediumType = string.Join(", ", names);
				}

				return _mediumType;
			}
		}
		private string _mediumType;
	}

	public enum ClipboardFormat : short
	{
		/// <summary>CF_TEXT</summary>
		TEXT            = 1,

		/// <summary>CF_BITMAP</summary>
		BITMAP          = 2,

		/// <summary>CF_METAFILEPICT</summary>
		METAFILEPICT    = 3,

		/// <summary>CF_SYLK</summary>
		SYLK            = 4,

		/// <summary>CF_DIF</summary>
		DIF             = 5,

		/// <summary>CF_TIFF</summary>
		TIFF            = 6,

		/// <summary>CF_OEMTEXT</summary>
		OEMTEXT         = 7,

		/// <summary>CF_DIB</summary>
		DIB             = 8,

		/// <summary>CF_PALETTE</summary>
		PALETTE         = 9,

		/// <summary>CF_PENDATA</summary>
		PENDATA         = 10,

		/// <summary>CF_RIFF</summary>
		RIFF            = 11,

		/// <summary>CF_WAVE</summary>
		WAVE            = 12,

		/// <summary>CF_UNICODETEXT</summary>
		UNICODETEXT     = 13,

		/// <summary>CF_ENHMETAFILE</summary>
		ENHMETAFILE     = 14,

		/// <summary>CF_HDROP</summary>
		HDROP           = 15,

		/// <summary>CF_LOCALE</summary>
		LOCALE          = 16,

		/// <summary>CF_DIBV5</summary>
		DIBV5           = 17,

		/// <summary>CF_OWNERDISPLAY</summary>
		OWNERDISPLAY    = 0x0080,

		/// <summary>CF_DSPTEXT</summary>
		DSPTEXT         = 0x0081,

		/// <summary>CF_DSPBITMAP</summary>
		DSPBITMAP       = 0x0082,

		/// <summary>CF_DSPMETAFILEPICT</summary>
		DSPMETAFILEPICT = 0x0083,

		/// <summary>CF_DSPENHMETAFILE</summary>
		DSPENHMETAFILE  = 0x008E,

		/// <summary>CF_PRIVATEFIRST</summary>
		PRIVATEFIRST    = 0x0200,

		/// <summary>CF_PRIVATELAST</summary>
		PRIVATELAST     = 0x02FF,

		/// <summary>CF_GDIOBJFIRST</summary>
		GDIOBJFIRST     = 0x0300,

		/// <summary>CF_GDIOBJLAST</summary>
		GDIOBJLAST      = 0x03FF,
	}

	public static class UserApi
	{
		public static string GetClipboardFormatName(short format)
		{
			var sb = new StringBuilder(256);
			var cch = Private.GetClipboardFormatName(format, sb, sb.Capacity);
			if (cch == 0)
				throw new Win32Exception();

			return sb.ToString();
		}

		private static class Private
		{
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern int GetClipboardFormatName(int format, StringBuilder szFormatName, int cchMaxCount);
		}
	}
}
