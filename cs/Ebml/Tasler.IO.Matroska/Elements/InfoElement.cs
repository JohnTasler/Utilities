namespace Tasler.IO.Matroska
{
	using System;
	using Tasler.IO.Ebml;

	public class InfoElement : MasterElement
	{
		#region Static Fields
		private static readonly ElementIdMap elementIdMap =
			new ElementIdMap(
				new ElementIdMapping<InfoElement, GuidElement>("SegmentFamily", 0x4444)
				{
					AssignValueAction = (p, e) => p.SegmentFamily = e.Value
				},
				new ElementIdMapping<InfoElement, DateElement>("DateUTC", 0x4461)
				{
					AssignValueAction = (p, e) => p.DateUTC = e.Value
				},
				new ElementIdMapping<InfoElement, FloatElement>("Duration", 0x4489)
				{
					AssignValueAction = (p, e) => p.Duration = e.Value
				},
				new ElementIdMapping<InfoElement, UnicodeStringElement>("MuxingApp", 0x4D80)
				{
					AssignValueAction = (p, e) => p.MuxingApp = e.Value
				},
				new ElementIdMapping<InfoElement, UnicodeStringElement>("WritingApp", 0x5741)
				{
					AssignValueAction = (p, e) => p.WritingApp = e.Value
				},
				new ElementIdMapping<InfoElement, ChapterTranslateElement>("ChapterTranslate", 0x6924)
				{
					AssignValueAction = (p, e) => p.ChapterTranslate = e
				},
				new ElementIdMapping<InfoElement, UnicodeStringElement>("SegmentFilename", 0x7384)
				{
					AssignValueAction = (p, e) => p.SegmentFilename = e.Value
				},
				new ElementIdMapping<InfoElement, GuidElement>("SegmentUID", 0x73A4)
				{
					AssignValueAction = (p, e) => p.SegmentUID = e.Value
				},
				new ElementIdMapping<InfoElement, UnicodeStringElement>("Title", 0x7BA9)
				{
					AssignValueAction = (p, e) => p.Title = e.Value
				},
				new ElementIdMapping<InfoElement, UnsignedIntegerElement>("TimecodeScale", 0x2AD7B1)
				{
					AssignDefaultAction = p => p.TimecodeScale = 1000000,
					AssignValueAction = (p, e) => p.TimecodeScale = e.Value
				},
				new ElementIdMapping<InfoElement, UnicodeStringElement>("PrevFilename", 0x3C83AB)
				{
					AssignValueAction = (p, e) => p.PrevFilename = e.Value
				},
				new ElementIdMapping<InfoElement, UnicodeStringElement>("NextFilename", 0x3E83BB)
				{
					AssignValueAction = (p, e) => p.NextFilename = e.Value
				},
				new ElementIdMapping<InfoElement, GuidElement>("PrevUID", 0x3CB923)
				{
					AssignValueAction = (p, e) => p.PrevUID = e.Value
				},
				new ElementIdMapping<InfoElement, GuidElement>("NextUID", 0x3EB923)
				{
					AssignValueAction = (p, e) => p.NextUID = e.Value
				}
			);
		#endregion Static Fields

		#region Properties
		public Guid? SegmentUID { get; private set; }
		public string SegmentFilename { get; private set; }
		public Guid? PrevUID { get; private set; }
		public string PrevFilename { get; private set; }
		public Guid? NextUID { get; private set; }
		public string NextFilename { get; private set; }
		public Guid? SegmentFamily { get; private set; }
		public ChapterTranslateElement ChapterTranslate { get; set; }
		public ulong TimecodeScale { get; private set; }
		public double Duration { get; private set; }
		public DateTime? DateUTC { get; private set; }
		public string Title { get; private set; }
		public string MuxingApp { get; private set; }
		public string WritingApp { get; private set; }

		#endregion Properties

		#region Overrides
		protected override ElementIdMap GetElementIdMap()
		{
			return elementIdMap;
		}
		#endregion Overrides
	}
}
