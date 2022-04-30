namespace Tasler.IO.Ebml
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Represents the most general information about an EBML element.
	/// </summary>
	public class Element : IElement
	{
		#region Properties

		public IReader Reader
		{
			get
			{
				return this.Parent.Reader;
			}
		}

		public IParent Parent { get; private set; }

		public long Position { get; private set; }

		public ulong Id { get; private set; }

		public ulong Size { get; private set; }

		public long DataOffset { get; private set; }

		public long DataPosition
		{
			get { return this.Position + this.DataOffset; }
		}

		public string ElementName
		{
			get { return this.elementName ?? (this.elementName = FormatHexElementName(this.Id)); }
			set { this.elementName = value; }
		}
		private string elementName;

		public IEnumerable<IParent> Ancestors
		{
			get
			{
				var parent = this.Parent;
				while (parent != null)
				{
					yield return parent;

					var element = parent as IElement;
					parent = (element != null) ? element.Parent : null;
				}
			}
		}
		#endregion Properties

		#region Methods

		public static string FormatHexElementName(ulong id)
		{
			var mask = 0xFF00000000000000;
			var shift = 56;

			// Find the first non-zero byte of the ID
			while ((id & mask) == 0)
			{
				mask >>= 8;
				shift -= 8;
			}

			// Format each subsequent byte
			var sb = new StringBuilder(32);
			while (mask != 0)
			{
				var idByte = (id & mask) >> shift;
				mask >>= 8;
				shift -= 8;
				sb.Append('[').Append(idByte.ToString("X2")).Append(']');
			}

			return sb.ToString();
		}

		public void Initialize(IParent parent, long position, ulong id, long idByteCount, string elementName)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			this.Parent = parent;
			this.Position = position;
			this.Id = id;
			this.ElementName = elementName;

			long sizeByteCount;
			this.Size = this.Reader.ReadSize(position + idByteCount, out sizeByteCount);
			this.DataOffset = idByteCount + sizeByteCount;

			this.OnInitialized();
		}

		#endregion Methods

		#region Overridables

		/// <summary>
		/// Called after the element has been initialized.
		/// </summary>
		protected virtual void OnInitialized()
		{
		}

		#endregion Overridables

		#region Overrides
		public override string ToString()
		{
			return string.Format("{0} 0x{1:X2}({1:N0}) bytes", this.ElementName, this.Size);
		}
		#endregion Overrides
	}
}
