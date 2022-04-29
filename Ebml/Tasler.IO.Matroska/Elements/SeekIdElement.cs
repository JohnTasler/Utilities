namespace Tasler.IO.Matroska
{
	using System.Linq;
	using Tasler.IO.Ebml;

	public class SeekIdElement : Element
	{
		public ulong Value { get; private set; }
		public string SeekIdName { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} Value=0x{1:X} SeekIdName={2}", base.ToString(), this.Value, this.SeekIdName);
		}

		protected override void OnInitialized()
		{
			long bytesRead;
			this.Value = this.Reader.ReadId(this.DataPosition, out bytesRead);

			var segmentElement = this.Ancestors.OfType<SegmentElement>().FirstOrDefault();
			if (segmentElement != null)
			{
				var elementIdMapping = segmentElement.FindElementMapping(this.Value);
				if (elementIdMapping != null)
					this.SeekIdName = elementIdMapping.ElementName;
			}

			if (this.SeekIdName == null)
				this.SeekIdName = FormatHexElementName(this.Value);
		}
	}
}
