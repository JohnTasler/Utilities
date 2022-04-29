namespace Tasler.IO.Ebml
{
	using System.IO;
	using System.Text;

	public class StringElement : Element
	{
		private const int bufferSize = 4096;

		public string Value { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} Value=\"{1}\"", base.ToString(), this.Value);
		}

		protected override void OnInitialized()
		{
			if (this.Size > int.MaxValue)
				throw new InvalidDataException("This EBML implementation only supports string values up to int.MaxValue in length.");

			var encoding = this.Encoding;

			var size = (int)this.Size;
			if (size < bufferSize)
			{
				var buffer = new byte[size];
				var position = this.DataPosition;
				var bytesRead = this.Reader.ReadBytes(position, buffer);
				this.Value = encoding.GetString(buffer, 0, bytesRead);
			}
			else
			{
				var sb = new StringBuilder(size);
				var buffer = new byte[bufferSize];
				var position = this.DataPosition;
				var endPosition = position + size;

				while (position < endPosition)
				{
					var bytesRead = this.Reader.ReadBytes(position, buffer);
					var value = encoding.GetString(buffer, 0, bytesRead);
					sb.Append(value);

					position += bytesRead;
				}

				this.Value = sb.ToString();
			}
		}

		protected virtual Encoding Encoding
		{
			get { return Encoding.ASCII; }
		}
	}
}
