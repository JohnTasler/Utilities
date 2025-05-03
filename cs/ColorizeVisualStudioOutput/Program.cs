using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Tasler.Utility.ColorizeVisualStudioOutput;

if (args.Length == 0 || IsHelpParameter(args[0]))
	Environment.Exit(ShowSyntax());

if (args[0] == "-v" || args[0] == "--version")
	Environment.Exit(ShowVersion());

try
{
	Colorizer.ProcessInputFile(args[0], args.Length > 1 ? args[1] : null);
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
}

bool IsHelpParameter(string parameter)
{
	return args[0] == "/?"
		|| args[0] == "-?"
		|| args[0] == "/h"
		|| args[0] == "-h"
		|| args[0] == "/help"
		|| args[0] == "-help"
		|| args[0] == "help"
		|| args[0] == "--help";
}

int ShowSyntax()
{
	//----+----1----+----2----+----3----+----4----+----5----+----6----+----7----+----8
	Console.WriteLine("""
		Colorizes Visual Studio debug output

		Syntax:
		  ColorizeVisualStudioOutput.exe -? | -h | --help
		  ColorizeVisualStudioOutput.exe -v | --version
		  ColorizeVisualStudioOutput.exe input_file [ output_file ]

		Where:
		  input_file:  the plain text file copied from the Visual Studio Output Window
		  output_file: the html file generated with colorization. If not specified, it
		               will default to the same as input_file but with the
		               extension changed to html.
		""");
	//----+----1----+----2----+----3----+----4----+----5----+----6----+----7----+----8

	return -1;
}

[RequiresAssemblyFiles("Calls System.Reflection.Assembly.Location")]
int ShowVersion()
{
	var v = FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location);
	Console.WriteLine(v);
	return 0;
}
