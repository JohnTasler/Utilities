namespace Tasler.IO.Ebml
{
	using System;
	using System.IO;

	public class FloatElement : Element
	{
		public double Value { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} Value={1:N}", base.ToString(), this.Value);
		}

		protected override void OnInitialized()
		{
			if (this.Size != 4 && this.Size != 8)
				throw new FormatException("This EBML implementation only supports float values of either 4 or 8 bytes in length.");

			// Read the bytes
			var buffer = new byte[this.Size];
			var bytesRead = this.Reader.ReadBytes(this.DataPosition, buffer);

			// Reverse the bytes from big-endian to little-endian
			Array.Reverse(buffer);

			// Create a MemoryStream on the byte array
			using (var stream = new MemoryStream(buffer))
			using (var reader = new BinaryReader(stream))
			{
				// Read the value
				if (buffer.Length == 4)
					this.Value = reader.ReadSingle();
				else
					this.Value = reader.ReadDouble();
			}
		}
	}
}
