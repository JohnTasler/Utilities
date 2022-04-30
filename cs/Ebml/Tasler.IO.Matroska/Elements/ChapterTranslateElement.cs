namespace Tasler.IO.Matroska
{
	using System.IO;
	using Tasler.IO.Ebml;

	public class ChapterTranslateElement : MasterElement
	{
		#region Static Fields
		private static readonly ElementIdMap elementIdLookup =
			new ElementIdMap(
				new ElementIdMapping<ChapterTranslateElement, BinaryElement>("ChapterTranslateId", 0x69A5)
				{
					AssignValueAction = (p, e) => p.ChapterTranslateID = e.Value,
				},
				new ElementIdMapping<ChapterTranslateElement, UnsignedIntegerElement>("ChapterTranslateCodec", 0x69BF)
				{
					AssignValueAction = (p, e) => p.ChapterTranslateCodec = e.Value,
				},
				new ElementIdMapping<ChapterTranslateElement, UnsignedIntegerElement>("ChapterTranslateEditionUID", 0x69FC)
				{
					AssignValueAction = (p, e) => p.ChapterTranslateEditionUID = e.Value
				}
			);
		#endregion Static Fields

		#region Properties
		public ulong? ChapterTranslateEditionUID { get; private set; }
		public ulong ChapterTranslateCodec { get; private set; }
		public Stream ChapterTranslateID { get; private set; }
		#endregion Properties

		#region Overrides
		protected override ElementIdMap GetElementIdMap()
		{
			return elementIdLookup;
		}
		#endregion Overrides
	}
}
