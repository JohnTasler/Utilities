namespace Tasler.IO.Ebml
{

	public class EbmlElement : MasterElement
	{
		#region Static Fields
		private static readonly ElementIdMap elementIdLookup = new ElementIdMap(
				new ElementIdMapping<EbmlElement, StringElement>("DocType", 0x4282)
				{
					AssignDefaultAction = p => p.DocType = "matroska",
					AssignValueAction = (p, e) => p.DocType = e.Value,
				},
				new ElementIdMapping<EbmlElement, UnsignedIntegerElement>("DocTypeReadVersion", 0x4285)
				{
					AssignDefaultAction = p => p.DocTypeReadVersion = 1,
					AssignValueAction = (p, e) => p.DocTypeReadVersion = e.Value,
				},
				new ElementIdMapping<EbmlElement, UnsignedIntegerElement>("EBMLVersion", 0x4286)
				{
					AssignDefaultAction = p => p.EBMLVersion = 1,
					AssignValueAction = (p, e) => p.EBMLVersion = e.Value,
				},
				new ElementIdMapping<EbmlElement, UnsignedIntegerElement>("DocTypeVersion", 0x4287)
				{
					AssignDefaultAction = p => p.DocTypeVersion = 1,
					AssignValueAction = (p, e) => p.DocTypeVersion = e.Value,
				},
				new ElementIdMapping<EbmlElement, UnsignedIntegerElement>("EBMLMaxIDLength", 0x42F2)
				{
					AssignDefaultAction = p => p.EBMLMaxIDLength = 4,
					AssignValueAction = (p, e) => p.EBMLMaxIDLength = e.Value,
				},
				new ElementIdMapping<EbmlElement, UnsignedIntegerElement>("EBMLMaxSizeLength", 0x42F3)
				{
					AssignDefaultAction = p => p.EBMLMaxSizeLength = 8,
					AssignValueAction = (p, e) => p.EBMLMaxSizeLength = e.Value,
				},
				new ElementIdMapping<EbmlElement, UnsignedIntegerElement>("EBMLReadVersion", 0x42F7)
				{
					AssignDefaultAction = p => p.EBMLReadVersion = 1,
					AssignValueAction = (p, e) => p.EBMLReadVersion = e.Value,
				}
			);
		#endregion Static Fields

		#region Properties
		public ulong EBMLVersion { get; private set; }
		public ulong EBMLReadVersion { get; private set; }
		public ulong EBMLMaxIDLength { get; private set; }
		public ulong EBMLMaxSizeLength { get; private set; }
		public string DocType { get; private set; }
		public ulong DocTypeVersion { get; private set; }
		public ulong DocTypeReadVersion { get; private set; }
		#endregion Properties

		#region Overrides
		protected override ElementIdMap GetElementIdMap()
		{
			return elementIdLookup;
		}
		#endregion Overrides
	}
}
