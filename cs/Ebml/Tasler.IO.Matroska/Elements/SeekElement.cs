namespace Tasler.IO.Matroska
{
	using Tasler.IO.Ebml;

	public class SeekElement : MasterElement
	{
		#region Static Fields
		private static readonly ElementIdMap elementIdMap =
			new ElementIdMap(
				new ElementIdMapping<SeekElement, SeekIdElement>("SeekId", 0x53AB)
				{
					AssignValueAction = (p, e) => { p.SeekId = e.Value; p.SeekIdName = e.SeekIdName; }
				},
				new ElementIdMapping<SeekElement, UnsignedIntegerElement>("SeekPosition", 0x53AC)
				{
					AssignValueAction = (p, e) => p.SeekPosition = e.Value
				}
			);
		#endregion Static Fields

		#region Properties
		public ulong SeekId { get; private set; }
		public string SeekIdName { get; private set; }
		public ulong SeekPosition { get; private set; }
		#endregion Properties

		#region Overrides
		protected override ElementIdMap GetElementIdMap()
		{
			return elementIdMap;
		}
		#endregion Overrides
	}
}
