namespace Tasler.IO.Ebml
{
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a fixed-size mapping of element id's to <see cref="ElementIdMapping"/>'s.
	/// </summary>
	public class ElementIdMap : IEnumerable<ElementIdMapping>, IEnumerable
	{
		#region Instance Fields
		private SortedList<ulong, ElementIdMapping> sortedList;
		#endregion Instance Fields

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ElementIdMap"/> class.
		/// </summary>
		/// <param name="values">The mappings with which to populate the map.</param>
		/// <seealso cref="ElementIdMapping"/>
		public ElementIdMap(params ElementIdMapping[] values)
		{
			var length = (values == null) ? 0 : values.Length;

			this.sortedList = new SortedList<ulong, ElementIdMapping>(length);

			for (var index = 0; index < length; ++index)
				this.sortedList.Add(values[index].Id, values[index]);
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Finds the element mapping for the specified element <paramref name="id"/>.
		/// </summary>
		/// <param name="id">The element ID.</param>
		/// <returns>The <see cref="ElementIdMapping"/> associated with the specified <paramref name="id"/>, if any;
		/// otherwise, <c>null</c>.</returns>
		public ElementIdMapping FindElementMapping(ulong id)
		{
			ElementIdMapping result;
			this.sortedList.TryGetValue(id, out result);
			return result;
		}
		#endregion Methods

		#region IEnumerable<ElementIdMapping> Members
		IEnumerator<ElementIdMapping> IEnumerable<ElementIdMapping>.GetEnumerator()
		{
			return this.sortedList.Values.GetEnumerator();
		}
		#endregion IEnumerable<ElementIdMapping> Members

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.sortedList.Values.GetEnumerator();
		}
		#endregion IEnumerable Members
	}
}
