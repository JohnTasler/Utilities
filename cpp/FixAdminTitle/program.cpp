#include "pch.h"

inline constexpr auto prefix = "[admin] "sv;
inline constexpr auto setTitleOpenEscape = "\x1B]0;"sv;
inline constexpr auto setTitleCloseEscape = "\x1B\x5C"sv;

void WriteOut(HANDLE fileHandle, std::string_view text)
{
	DWORD numberOfBytesWritten{};
	WriteFile(fileHandle, text.data(), static_cast<DWORD>(text.size()), &numberOfBytesWritten, nullptr);
}

int run()
{
	if (auto isUserAdmin = IsUserAnAdmin())
	{
		std::array<char, MAX_PATH> titleArray{};
		GetConsoleOriginalTitleA(titleArray.data(), static_cast<DWORD>(titleArray.size()));
		std::string_view title{ titleArray.data() };

		std::string newTitle;
		newTitle.reserve(setTitleOpenEscape.length() + prefix.length() + title.length() + setTitleCloseEscape.length());
		newTitle
			.append(setTitleOpenEscape)
			.append(prefix)
			.append(title)
			.append(setTitleCloseEscape);

		SetConsoleOutputCP(CP_UTF8);
		auto out = GetStdHandle(STD_OUTPUT_HANDLE);
		WriteOut(out, newTitle.c_str());

		return true;
	}

	return false;
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
