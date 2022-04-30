namespace Tasler.IO.Matroska
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Tasler.IO.Ebml;

	public class MatroskaReader : EbmlReader
	{
		#region Static Fields
		private static readonly ElementIdMap[] elementIdMaps =
		{
			new ElementIdMap(new ElementIdMapping<SegmentElement>("Segment", 0x18538067))
		};
		#endregion Static Fields
	
		public MatroskaReader(Stream stream)
			: base(stream)
		{
		}

		protected override IEnumerable<ElementIdMap> GetElementIdMaps()
		{
			return base.GetElementIdMaps().Concat(elementIdMaps);
		}
	}
}
