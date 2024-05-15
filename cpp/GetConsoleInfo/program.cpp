#include "pch.h"

inline constexpr auto c_lineEnd = "\n"sv;
inline constexpr auto c_prefix = "[admin] "sv;

void WriteOut(HANDLE fileHandle, std::string_view text)
{
	DWORD numberOfBytesWritten{};
	WriteFile(fileHandle, text.data(), static_cast<DWORD>(text.size()), &numberOfBytesWritten, nullptr);
}

void WriteLine(HANDLE fileHandle, std::string_view text)
{
	WriteOut(fileHandle, text);

	DWORD numberOfBytesWritten{};
	WriteFile(fileHandle, c_lineEnd.data(), static_cast<DWORD>(c_lineEnd.size()), &numberOfBytesWritten, nullptr);
}

int run()
{
	SetConsoleOutputCP(CP_UTF8);

	auto out = GetStdHandle(STD_OUTPUT_HANDLE);

	auto isUserAnAdmin = IsUserAnAdmin();
	if (isUserAnAdmin)
	{
		WriteOut(out, c_prefix);
	}

	std::array<char, MAX_PATH> titleArray{};
	GetConsoleOriginalTitleA(titleArray.data(), static_cast<DWORD>(titleArray.size()));
	std::string_view title{ titleArray.data() };

	WriteLine(out, title);

	return isUserAnAdmin;
}

int main()
{
#ifdef _DEBUG
	while (!IsDebuggerPresent())
	{
		Sleep(333);
	}
#endif

	auto result = run();

	if (IsDebuggerPresent())
	{
		(void)_getch_nolock();
	}

	return result;
}
