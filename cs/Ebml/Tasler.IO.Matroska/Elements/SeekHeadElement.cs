namespace Tasler.IO.Matroska
{
	using Tasler.IO.Ebml;

	public class SeekHeadElement : MasterElement
	{
		#region Static Fields
		private static readonly ElementIdMap elementIdMap =
			new ElementIdMap(new ElementIdMapping<SeekElement>("Seek", 0x4DBB));
		#endregion Static Fields

		#region Overrides
		protected override ElementIdMap GetElementIdMap()
		{
			return elementIdMap;
		}
		#endregion Overrides
	}
}
