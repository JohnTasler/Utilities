namespace Tasler.IO.Ebml
{

	public class UnsignedIntegerElement : Element
	{
		public ulong Value { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} Value=0x{1:X} ({1:N0})", base.ToString(), this.Value);
		}

		protected override void OnInitialized()
		{
			var buffer = new byte[this.Size];
			var bytesRead = this.Reader.ReadBytes(this.DataPosition, buffer);

			ulong value = buffer[0];
			for (var i = 1; i < bytesRead; ++i)
			{
				value <<= 8;
				value |= buffer[i];
			}

			this.Value = value;
		}
	}
}
