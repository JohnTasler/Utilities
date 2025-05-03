using System.Diagnostics;
using System.Text.RegularExpressions;

[assembly: System.Reflection.AssemblyFlags(System.Reflection.AssemblyNameFlags.PublicKey)]


namespace Tasler.Utility.ColorizeVisualStudioOutput;

internal static partial class Colorizer
{
	public static void ProcessInputFile(string inputFile, string? outputFile = null)
	{
		Console.WriteLine($"Reading: {inputFile}");

		outputFile ??= Path.ChangeExtension(inputFile, "html");
		Console.WriteLine($"Writing: {outputFile}");

		using var reader = new StreamReader(inputFile);
		using var writer = new StreamWriter(outputFile);

		ColorizeInputToOutput(reader, writer);
	}

	private static void ColorizeInputToOutput(TextReader inputReader, TextWriter outputWriter)
	{
		outputWriter.WriteLine(c_htmlHeader);

		int lineIndex = 0;
		for (var line = inputReader.ReadLine(); line is not null; line = inputReader.ReadLine())
		{
			++lineIndex;
			bool foundMatch = false;
			foreach (var pair in Classifications)
			{
				var match = pair.Regexp.Match(line);
				foundMatch = match.Success;
				if (!foundMatch)
					continue;

				// Write the decorated line
				var outputLine = $"<span class=\"{pair.StyleClass}\">{line}</span>";
				outputWriter.WriteLine(outputLine);
				//Debug.WriteLine($"{lineIndex}  {outputLine}");
				break;
			}

			if (!foundMatch)
			{
				// Write the line out
				outputWriter.WriteLine(line);
				//Debug.WriteLine($"{lineIndex}  {line}");
			}
		}

		outputWriter.WriteLine(c_htmlTrailer);
	}

	private const RegexOptions c_regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture;

	[GeneratedRegex(@"(\W|^)^(?!.*warning\s(BC|CS|CA)\d+:).*((?<!/)error|fail|crit|failed|exception)[^\w\.\-\+]", c_regexOptions)]
	private static partial Regex Create1();

	[GeneratedRegex(@"(exception:|stack trace:)", c_regexOptions)]
	private static partial Regex Create2();

	[GeneratedRegex(@"^\s+at\s", c_regexOptions)]
	private static partial Regex Create3();

	[GeneratedRegex(@"(\W|^)(warning|warn)\W", c_regexOptions)]
	private static partial Regex Create4();

	[GeneratedRegex(@"(\W|^)(information|info)\W", c_regexOptions)]
	private static partial Regex Create5();

	[GeneratedRegex(@"Could not find", c_regexOptions)]
	private static partial Regex Create6();

	[GeneratedRegex(@"failed", c_regexOptions)]
	private static partial Regex Create7();

	[GeneratedRegex(@"Symbols loaded", c_regexOptions)]
	private static partial Regex Create8();

	[GeneratedRegex(@": Loaded '", c_regexOptions)]
	private static partial Regex Create9();

	[GeneratedRegex(@"The thread .+ has exited with", c_regexOptions)]
	private static partial Regex Create10();

	[GeneratedRegex(@"The program .+ has exited with", c_regexOptions)]
	private static partial Regex Create11();

	private static readonly (Regex Regexp, string StyleClass)[] Classifications =
	[
		( Create1(), "error" ),
		( Create2(), "error" ),
		( Create3(), "error" ),
		( Create4(), "warning" ),
		( Create5(), "info" ),
		( Create6(), "error" ),
		( Create7(), "error" ),
		( Create8(), "symbol" ),
		( Create9(), "module" ),
		( Create10(), "threadExit" ),
		( Create11(), "processExit" ),
	];
}

