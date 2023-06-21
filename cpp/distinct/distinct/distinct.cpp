#include "pch.h"

int main()
{
	std::map<std::string, size_t> lines;
	std::map<size_t, std::string_view> orderedByIndex;

	size_t index = 0;
	for (std::string line; std::getline(std::cin, line); ++index)
	{
		if (lines.end() != lines.find(line))
		{
			lines.emplace(line, index);
			orderedByIndex.emplace(index, line);
		}
	}

	for (auto const& item : orderedByIndex)
	{
		std::cout << item.second << std::endl;
	}

	return 0;
}
