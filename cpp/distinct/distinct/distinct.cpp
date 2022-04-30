#include "pch.h"

int main()
{
	std::set<std::string> lines;

	for (std::string line; std::getline(std::cin, line); )
	{
		lines.emplace(line);
	}

	for (auto& line : lines)
	{
		std::cout << line << std::endl;
	}

	return 0;
}
