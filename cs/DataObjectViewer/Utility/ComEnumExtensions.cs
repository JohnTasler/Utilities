namespace DataObjectViewer.Utility
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;

	public static class ComEnumExtensions
	{
		public static IEnumerable<FORMATETC> AsEnumerable(this IEnumFORMATETC enumerator, int blockSize)
		{
			// Allocate a block of items
			var items = new FORMATETC[blockSize];
			var hr = 0;

			do
			{
				// Fetch the next block of items
				var fetched = new int[1];
				hr = enumerator.Next(items.Length, items, fetched);

				if (hr == 0 || hr == 1)
				{
					for (int index = 0; index < fetched[0]; ++index)
						yield return items[index];
				}
				else
				{
					throw Marshal.GetExceptionForHR(hr);
				}

			} while (hr == 0);
		}
	}
}
