namespace Tasler.IO.Ebml
{
	using System;

	public class DateElement : Element
	{
		private static readonly DateTime millennium = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public DateTime Value { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} Value={1}", base.ToString(), this.Value);
		}

		protected override void OnInitialized()
		{
			if (this.Size != 8)
				throw new FormatException("This EBML implementation only supports date values of 8 bytes in length.");

			var buffer = new byte[this.Size];
			var bytesRead = this.Reader.ReadBytes(this.DataPosition, buffer);
			ulong value = ulong.MaxValue;
			while (bytesRead-- > 0)
			{
				value <<= 8;
				value &= 0xFFFFFFFFFFFFFF00;
				value |= buffer[bytesRead];
			}

			var nanoseconds = unchecked((long)value);
			var ticks = nanoseconds / 100;
			var date = millennium;
			date.AddTicks(ticks);
			this.Value = date;
		}
	}
}
