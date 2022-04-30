namespace Tasler.IO.Ebml
{

	public static class ElementFactory
	{
		#region Static Fields
		private static readonly ElementIdMap globalElementIdMap =
			new ElementIdMap(
				new ElementIdMapping<BinaryElement>("CRC-32", 0xBF),
				new ElementIdMapping<BinaryElement>("Void", 0xEC)
			);
		#endregion Static Fields

		#region Methods
		public static IElement CreateElement(IParent parent, long position)
		{
			// Read the next id
			long idByteCount;
			ulong id = parent.Reader.ReadId(position, out idByteCount);

			// Get the ElementIdMapping from the parent
			var elementIdMapping = parent.FindElementMapping(id) ?? globalElementIdMap.FindElementMapping(id);

			// Construct and initialize the element
			var element = (elementIdMapping != null) ? elementIdMapping.CreateElement() : new Element();
			element.Initialize(parent, position, id, idByteCount, elementIdMapping.ElementName);

			// Return the created element
			return element;
		}
		#endregion Methods
	}
}
