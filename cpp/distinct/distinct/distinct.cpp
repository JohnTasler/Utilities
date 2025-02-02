#include "pch.h"

using namespace std::literals;

int main(int argc, char* args[])
{
	if (argc < 2 || argc > 3)
	{
		std::println("\n"
			"Reads an input text file and outputs the distinct lines of the file.\n\n"
			"Syntax:\n"
			"    distinct input_filename [output_filename]\n"
			"Where :\n"
			"    input_filename  is the text file to read lines from\n"
			"    output_filename is the optional output name to write to\n\n"
			"If the output_file_name is omitted, the output will be written to the console.\n"
			"If the output_file_name does not specify any directory information (either\n"
			"relative or absolute), the file will be created in the same directory as the\n"
			"input_filename.The lines will be output in the same order as they are read,\n"
			"but with duplicates removed."sv
		);
		return 1;
	}

	std::filesystem::path input_file_path{ args[1] };
	std::filesystem::directory_entry input_directory_entry{ std::string_view{ args[1] } };
	if (!input_directory_entry.exists())
	{
		std::println(std::cerr, "Specified file does not exist:\n    {}"sv, input_directory_entry.path().string());
	}
	
	std::fstream input_file_stream{};
	input_file_stream.open(input_file_path, std::ios_base::in);
	if (!input_file_stream.is_open())
	{
		std::println(std::cerr, "Unable to open the specified input_filename:\n    {}"sv, input_file_path.string());
		return 2;
	}

	std::ostream* output_stream = &std::cout;
	std::fstream output_file_stream;
	if (argc == 3)
	{
		std::filesystem::path output_file_path{ args[2] };
		std::filesystem::directory_entry output_directory_entry{ output_file_path };
		if (!output_directory_entry.path().has_parent_path())
		{
			output_file_path = { input_directory_entry.path().parent_path().append(output_file_path.string())};
			output_directory_entry.replace_filename(output_file_path);
		}

		output_file_stream.open(output_file_path, std::ios_base::out | std::ios_base::trunc);
		if (!output_file_stream.is_open())
		{
			std::println(std::cerr, "Unable to create the specified output_filename:\n    {}"sv, output_file_path.string());
			return 4;
		}

		output_stream = &output_file_stream;
	}

	std::unordered_set<std::string> lines{};
	std::vector<std::string_view> orderedByIndex{};

	size_t index = 0;
	for (std::string line; std::getline(input_file_stream, line); ++index)
	{
		if (!lines.contains(line))
		{
			auto const& pair = lines.emplace(std::move(line));
			orderedByIndex.emplace_back(*pair.first);
		}
	}

	char lastCharacter = '\0';
	input_file_stream.clear();
	input_file_stream.seekg(-1, std::ios::end).get(lastCharacter);
	input_file_stream.close();
	bool endsWithNewLine = lastCharacter == '\n';

	std::for_each(orderedByIndex.begin(), orderedByIndex.end() - 1,
		[&output_stream](auto&& item)
		{
			output_stream->write(item.data(), static_cast<std::streamsize>(item.length())).put('\n');
		});

	output_stream->write(orderedByIndex.rbegin()->data(), orderedByIndex.rbegin()->size());
	if (endsWithNewLine)
	{
		output_stream->put('\n');
	}

	output_stream->flush();

	std::println("File         : {}"sv, input_file_path.string());
	std::println("Lines read   : {}"sv, index);
	std::println("Lines copied : {}"sv, lines.size());
	std::println("Lines skipped: {} duplicates"sv, index - lines.size());

	return 0;
}
