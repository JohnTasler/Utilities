namespace DataObjectViewer.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices.ComTypes;
	using DataObjectViewer.Properties;
	using Tasler.ComponentModel;
	using Tasler.Interop.User;

	public class FormatViewModel : Child<DataObjectViewModel>, IModelContainer<FORMATETC>
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
						names.Add(Strings.DeviceAspectContent);
					if (value.HasFlag(DVASPECT.DVASPECT_THUMBNAIL))
						names.Add(Strings.DeviceAspectThumbnail);
					if (value.HasFlag(DVASPECT.DVASPECT_ICON))
						names.Add(Strings.DeviceAspectIcon);
					if (value.HasFlag(DVASPECT.DVASPECT_DOCPRINT))
						names.Add(Strings.DeviceAspectDocPrint);

					_aspect = string.Join(" | ", names);
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
							names.Add(Strings.MediaTypeHGLOBAL);
						if (value.HasFlag(TYMED.TYMED_FILE))
							names.Add(Strings.MediaTypeFile);
						if (value.HasFlag(TYMED.TYMED_ISTREAM))
							names.Add(Strings.MediaTypeIStream);
						if (value.HasFlag(TYMED.TYMED_ISTORAGE))
							names.Add(Strings.MediaTypeIStorage);
						if (value.HasFlag(TYMED.TYMED_GDI))
							names.Add(Strings.MediaTypeGDI);
						if (value.HasFlag(TYMED.TYMED_MFPICT))
							names.Add(Strings.MediaTypeMetafilePict);
						if (value.HasFlag(TYMED.TYMED_ENHMF))
							names.Add(Strings.MediaTypeEnhancedMetafile);
					}

					_mediumType = string.Join(", ", names);
				}

				return _mediumType;
			}
		}
		private string _mediumType;
	}
}
