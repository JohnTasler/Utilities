/////////////////////////////////////////////////////////////////////////////
// FileProcessor.h
//

#include <conio.h>
#include <wtypes.h>

#include <experimental/generator>
#include <filesystem>
#include <string>
#include <string_view>
#include <system_error>

using namespace std::literals;

/////////////////////////////////////////////////////////////////////////////
//
template <class Derived,
	bool autoExpand = true,
	bool includeHidden = false,
	bool includeSystem = false,
	bool includeSubDirectories = false>
class file_processor
{
// Types
protected:
	using file_processor_base = file_processor<Derived, autoExpand, includeHidden, includeSystem, includeSubDirectories>;

// Construction
public:
	file_processor(FILE* fileOut = stdout, FILE* fileErr = stderr)
		: m_fileOut(fileOut ? fileOut : stdout)
		, m_fileErr(fileErr ? fileErr : stderr)
	{
	}

// Return/exit codes
public:
	enum
	{
		eSuccess     = 0,
		eNoFileSpecs = 1,
		eSyntaxError = 2,
		eFileError   = 3,
		eUnknown     = 4,
	};

// Properties
public:
	FILE* GetFileOut()  const { return m_fileOut; }
	FILE* GetFileErr()  const { return m_fileErr; }
	int   GetArgCount() const { return m_argc; }
	std::wstring_view GetArg(int index) const { return m_argv[index]; }

// Operations
public:
	int Main(int argc, wchar_t* argv[]) noexcept
	{
		m_argc = argc;
		m_argv = argv;

		int result{};
		try
		{
			result = derived().DoMain();
		}
		catch (std::filesystem::filesystem_error const& ex)
		{
			fprintf(GetFileErr(), ex.what(), ex.path1().native());
			fprintf(GetFileErr(), "\n");
			result = ex.code().value();
		}
		catch (std::exception const& ex)
		{
			fprintf(GetFileErr(), ex.what());
			fprintf(GetFileErr(), "\n");
			result = eUnknown;
		}
		catch (...)
		{
			fwprintf(GetFileErr(), L"unknown error\n");
			return eUnknown;
		}

		if (IsDebuggerPresent())
		{
			fwprintf(this->GetFileOut(), L"\nPress any key to continue . . . ");
			(void)_getch();
		}

		return result;
	}

	int DoMain()
	{
		// Show syntax if no files were specified
		if (m_argc == 1)
		{
			derived().Syntax(L"No files specified.\n");
			return eNoFileSpecs;
		}

		// Process the switches
		int nResult = derived().ProcessSwitches();
		if (nResult > 0)
			return nResult;

		// Process the fileSpecs
		return derived().ProcessFileSpecs();
	}

	int SyntaxV(std::wstring_view const message, va_list args)
	{
		// Display the specified message
		if (message.data() != 0)
		{
			fputwc(L'\n', stderr);
			vfwprintf(stderr, message.data(), args);
			fputwc(L'\n', stderr);
		}

		// Display the syntax
		fputws(derived().GetSyntax().data(), stderr);

		// Indicate a syntax error
		return derived().eSyntaxError;
	}

	int Syntax(std::wstring_view const message = nullptr, ...)
	{
		va_list args;
		va_start(args, message);
		int nResult = derived().SyntaxV(message, args);
		va_end(args);
		return nResult;
	}

	void ReportFileError(std::wstring_view const& fileSpec)
	{
		std::string fmt;
		switch (errno)
		{
		case ENOENT:
			fmt = "File specification could not be matched: %ls\n"sv;
			break;
		case EINVAL:
			fmt = "Invalid filename specification: %ls\n"sv;
			break;
		default:
			fmt = "Unknown file error: %ls\n"sv;
		}

		std::filesystem::filesystem_error error
		{
			fmt,
			fileSpec,
			{ errno, std::system_category() }
		};

		throw error;
	}

// Overridables
protected:
	std::wstring_view GetSyntax()
	{
		return L""sv;
	}

	int ProcessSwitches()
	{
		// Iterate through the parameters, processing each switch
		for (int i = 1; i < derived().GetArgCount(); ++i)
		{
			wchar_t firstChar = derived().GetArg(i)[0];
			if (firstChar == L'/' || firstChar == L'-')
			{
				std::wstring_view commandSwitch{ derived().GetArg(i).substr(1) };

				// Make an upper-case copy of the switch
				std::wstring commandSwitchUpper(commandSwitch);
				_wcsupr_s(commandSwitchUpper.data(), commandSwitchUpper.length() + 1);

				// Process the switch
				int nResult = derived().ProcessSwitch(i, commandSwitch, commandSwitchUpper);
				if (nResult > 0)
					return nResult;
			}
		}

		// Indicate success
		return derived().eSuccess;
	}

	int ProcessSwitch(int index, std::wstring_view const& switchString, std::wstring_view const& switchUpper)
	{
		// Check for common help switches
		if (switchString[0] == L'?')
			return derived().ProcessHelp(index, switchString, switchUpper);
		if (switchUpper == L"HELP")
			return derived().ProcessHelp(index, switchString, switchUpper);
		return 0;
	}

	int ProcessHelp(int index, std::wstring_view const& switchString, std::wstring_view const& switchUpper)
	{
		return derived().Syntax();
	}

	int ProcessFileSpecs()
	{
		// Iterate thru the fileSpecs
		int nResult = derived().eSuccess;
		for (int i = 1; i < derived().GetArgCount(); ++i)
		{
			wchar_t firstChar = derived().GetArg(i)[0];
			if (firstChar != L'/' && firstChar != L'-')
			{
				// Get the fileSpec
				auto fileSpec = derived().GetArg(i);

				// Expand wildcards, if specified
				if constexpr (autoExpand)
				{
					for (auto const& path : derived().GetExpandedFileNames(fileSpec))
					{
						// Process the file
						if (auto nResult = derived().ProcessFileSpec(i, path); nResult != eSuccess)
						{
							break;
						}
					}
				}
				else
				{
					// Process the fileSpec without expanding it
					nResult = derived().ProcessFileSpec(i, fileSpec);
				}
			}
		}

		// Return the last result
		return nResult;
	}

	int ProcessFileSpec(int index, std::wstring_view const& fileSpec)
	{
		fwprintf(derived().GetFileOut(), L"%d) %s\n", index, fileSpec.data());
		return derived().eSuccess;
	}

private:
	inline Derived& derived() { return *static_cast<Derived*>(this); }
	inline static constexpr uint32_t exclude_attributes = 0
		| (includeHidden ? 0 : _A_HIDDEN)
		| (includeSystem ? 0 : _A_SYSTEM)
		| (includeSubDirectories ? 0 : _A_SUBDIR);

private:
	std::experimental::generator<std::wstring_view> GetExpandedFileNames(std::wstring_view const& fileSpec)
	{
		// Split the path information
		wchar_t drive[_MAX_DRIVE * 2];
		wchar_t dir  [_MAX_DIR   * 2];
		_wsplitpath_s(fileSpec.data(),
			drive, _countof(drive),
			dir, _countof(dir),
			nullptr, 0, nullptr, 0);

		// Find the first match, if any
		_wfinddata_t findData{};
		if (auto findHandle = _wfindfirst(fileSpec.data(), &findData); findHandle == -1)
		{
			ReportFileError(fileSpec.data());
		}
		else
		{
			// Process matching filename and iterate
			int find;
			do
			{
				// Don't process hidden, system, or directories as specified in template arguments
				if (!(findData.attrib & exclude_attributes))
				{
					// Create the full filepath
					wchar_t szFullPath[_MAX_PATH * 2];
					_wmakepath_s(szFullPath, drive, dir, findData.name, nullptr);

					// Process the file
					co_yield szFullPath;
				}

				// Get the next matching fileSpec
				find = _wfindnext(findHandle, &findData);

			} while (find != -1);

			// Close the file find handler
			_findclose(findHandle);
		}
	}

// Data Members
protected:
	FILE*  m_fileOut{};
	FILE*  m_fileErr{};
	int    m_argc{};
	wchar_t** m_argv{};
};

