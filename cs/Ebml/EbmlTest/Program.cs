namespace EbmlTest
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using Tasler.IO.Ebml;
	using Tasler.IO.Matroska;

	class Program
	{
		private int indentLevel;

		private static int Main(string[] args)
		{
			var instance = new Program();
			var result = instance.Run(args);

			if (Debugger.IsAttached)
			{
				Console.Write("Press any key to continue . . .");
				Console.ReadKey();
			}

			return result;
		}

		private int Run(string[] args)
		{
			if (args.Length != 1)
				return this.ShowSyntax();

			var fileName = args[0];

			using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var reader = new MatroskaReader(stream);
				this.IterateChildren(reader);
			}

			return 0;
		}

		private void IterateChildren(IParent parent)
		{
			var indent = new string(' ', 4 * this.indentLevel);

			foreach (var child in parent.Children)
			{
				Console.WriteLine(indent + child.ToString());

				var asParent = child as IParent;
				if (asParent != null)
				{
//					Console.WriteLine(indent + '{');

					var children = asParent.Children;
					if (children.Any())
					{
						++this.indentLevel;
						this.IterateChildren(asParent);
						--this.indentLevel;
					}

//					Console.WriteLine(indent + '}');
				}
			}
		}

		private int ShowSyntax()
		{
			Console.WriteLine("SYNTAX:");
			Console.WriteLine("    EbmlTest filename");
			return 1;
		}
	}
}
