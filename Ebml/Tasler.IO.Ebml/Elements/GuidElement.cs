namespace Tasler.IO.Ebml
{
	using System;
	using System.IO;

	public class GuidElement : Element
	{
		public Guid Value { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} Value={1}", base.ToString(), this.Value);
		}

		protected override void OnInitialized()
		{
			if (this.Size != 16)
				throw new InvalidDataException("This element is required to be 128 bits.");

			var buffer = new byte[this.Size];
			this.Reader.ReadBytes(this.DataPosition, buffer);

			this.Value = new Guid(buffer);
		}
	}
}
