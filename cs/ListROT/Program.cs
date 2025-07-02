using System.Diagnostics;
using Tasler.Interop.Com;

namespace ListROT;

class Program
{
	[MTAThread]
	static void Main(string[] args)
	{
		using (var rot = new RunningObjectTable())
		{
			var bindCtx = ComApi.CreateBindCtx();

			var monikers = rot.ToArray();
			Console.WriteLine("{0} Items in the Running Object Table:", monikers.Length);

			var maxDigits = (int)Math.Truncate(Math.Log10(monikers.Length)) + 1;
			var digitsFormatString = "D" + maxDigits;

			var index = 0;
			var displayName = string.Empty;
			var displayIndex = string.Empty;
			foreach (var moniker in monikers)
			{
				using (new ComPtr<IMoniker>(moniker))
				{
					++index;
					displayIndex = index.ToString(digitsFormatString);

					displayName = moniker.GetDisplayName(bindCtx, null);
				}

				Console.WriteLine("  [{0}] {1}", displayIndex, displayName);
			}
		}

		if (Debugger.IsAttached)
		{
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}
