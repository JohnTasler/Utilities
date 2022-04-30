using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace FixDpi
{
	class Program
	{
		string[] _args;
		int _fileCount;
		int _skipCount;

		static int Main(string[] args)
		{
			var program = new Program(args);
			var result = program.Run();

			var format = program._fileCount == 1 ? "1 file was processed." : "{0} files were processed.";
			Console.WriteLine(format, program._fileCount);

			format = program._skipCount == 1 ? "1 file was skipped." : "{0} files were skipped.";
			Console.WriteLine(format, program._skipCount);

			return result;
		}

		Program(string[] args)
		{
			_args = args;
		}

		int Run()
		{
			if (_args.Length < 1 || string.IsNullOrWhiteSpace(_args[0]))
			{
				Console.WriteLine("No image file specified.");
				return 1;
			}

			var directoryName = Path.GetDirectoryName(_args[0]);
			var path = Path.GetFullPath(string.IsNullOrWhiteSpace(directoryName) ? "." : directoryName);
			var fileName = Path.GetFileName(_args[0]);

			foreach (var file in Directory.EnumerateFiles(path, fileName))
			{
				try
				{
					if (this.ProcessFile(file))
						++_fileCount;
					else
						++_skipCount;
				}
				catch (Exception ex)
				{
					Console.WriteLine("ERROR: Exception occurred while processing {0}\n{1}", file, ex.ToString());
				}
			}

			return 0;
		}

		bool ProcessFile(string fileName)
		{
			// Create an Image object to load the specified filename
			using (var image = new Bitmap(fileName))
			{
				// Skip the file if resolution is already 72x72 or 96x96
				if (image.HorizontalResolution == image.VerticalResolution && (image.VerticalResolution == 72 || image.VerticalResolution == 96))
					return false;

				Console.WriteLine("{0} {1}x{2}", fileName, image.HorizontalResolution, image.VerticalResolution);

				var newDirectory = Path.Combine(Path.GetDirectoryName(fileName), "FixedDpi");
				if (!Directory.Exists(newDirectory))
					Directory.CreateDirectory(newDirectory);

				// Remove any properties related to resolution DPI
				foreach (var id in new[] { 0x011A, 0x011B, 0x0128})
					if (image.PropertyIdList.Any(i => i == id))
						image.RemovePropertyItem(id);

				var newFileName = Path.Combine(newDirectory, Path.GetFileName(fileName));
				var imageClone = image;
//				using (var imageClone = (Bitmap)image.Clone())
				{
					imageClone.SetResolution(72, 72);
					imageClone.Save(newFileName, ImageFormat.Jpeg);
				}
			}

			Console.WriteLine();
			return true;
		}
	}

}
