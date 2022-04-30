namespace Tasler.IO.Matroska
{
	using Tasler.IO.Ebml;

	public class SegmentElement : MasterElement
	{
		#region Static Fields
		private static readonly ElementIdMap elementIdMap =
			new ElementIdMap(
				new ElementIdMapping<SeekHeadElement>("SeekHead", 0x114D9B74),
				new ElementIdMapping<InfoElement>("Info", 0x1549A966),

				new ElementIdMapping<Element>("Cluster", 0x1F43B675),
				new ElementIdMapping<Element>("Tracks", 0x1654AE6B),
				new ElementIdMapping<Element>("Cues", 0x1C53BB6B),
				new ElementIdMapping<Element>("Attachments", 0x1941A469),
				new ElementIdMapping<Element>("Chapters", 0x1043A770)
				);
		#endregion Static Fields

		#region Overrides
		protected override ElementIdMap GetElementIdMap()
		{
			return elementIdMap;
		}
		#endregion Overrides
	}
}
