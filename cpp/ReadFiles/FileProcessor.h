/////////////////////////////////////////////////////////////////////////////
// FileProcessor.h
//

#include <conio.h>
#include <stdio.h>
#include <stdlib.h>
#include <io.h>
#include <errno.h>
#include <tchar.h>
#include <malloc.h>
#include <wtypes.h>

#include <string>
#include <string_view>

/////////////////////////////////////////////////////////////////////////////
//
template <class Derived, bool AutoExpand = true>
class file_processor
{
// Types
protected:
	using file_processor_base = file_processor<Derived, AutoExpand>;

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
		eNoFilespecs = 1,
		eSyntaxError = 2,
		eFileError   = 3,
	};

// Properties
public:
	FILE*  GetFileOut()      const { return m_fileOut; }
	FILE*  GetFileErr()      const { return m_fileErr; }
	int    GetArgCount()     const { return m_argc; }
	std::wstring_view GetArg(int index) const { return m_argv[index]; }

// Operations
public:
	int Main(int argc, wchar_t* argv[])
	{
		m_argc = argc;
		m_argv = argv;

		int result = derived().DoMain();

		if (IsDebuggerPresent())
		{
			_ftprintf(this->GetFileOut(), L"\nPress any key to continue . . . ");
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
			return eNoFilespecs;
		}

		// Process the switches
		int nResult = derived().ProcessSwitches();
		if (nResult > 0)
			return nResult;

		// Process the filespecs
		return derived().ProcessFilespecs();
	}
	int SyntaxV(PCWSTR pszMessage, va_list args)
	{
		// Display the specified message
		if (pszMessage != nullptr)
		{
			_fputtc(L'\n', stderr);
			_vftprintf(stderr, pszMessage, args);
			_fputtc(L'\n', stderr);
		}

		// Display the syntax
		_fputts(derived().GetSyntax(), stderr);

		// Indicate a syntax error
		return derived().eSyntaxError;
	}
	int Syntax(PCWSTR pszMessage = nullptr, ...)
	{
		va_list args;
		va_start(args, pszMessage);
		int nResult = derived().SyntaxV(pszMessage, args);
		va_end(args);
		return nResult;
	}
	int ReportFileError(PCWSTR pszFilespec)
	{
		PCWSTR pszFmt;
		switch (errno)
		{
		case ENOENT:
			pszFmt = L"File specification could not be matched: %s\n";
			break;
		case EINVAL:
			pszFmt = L"Invalid filename specification: %s\n";
			break;
		default:
			pszFmt = L"Unknown file error: %s\n";
		}

		_ftprintf(derived().GetFileErr(), pszFmt, pszFilespec);
		_fputts(L"\n", derived().GetFileErr());
		return derived().eFileError;
	}


// Overridables
protected:
	PCWSTR GetSyntax()
	{
		return L"Syntax";
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
				int nResult = derived().ProcessSwitch(i, commandSwitch.data(), commandSwitchUpper.c_str());
				if (nResult > 0)
					return nResult;
			}
		}

		// Indicate success
		return derived().eSuccess;
	}
	int ProcessSwitch(int index, PCWSTR pszSwitch, PCWSTR pszSwitchUpper)
	{
		// Check for common help switches
		if (pszSwitch[0] == L'?')
			return derived().ProcessHelp(index, pszSwitch, pszSwitchUpper);
		if (0 == _tcscmp(pszSwitchUpper, L"HELP"))
			return derived().ProcessHelp(index, pszSwitch, pszSwitchUpper);
		return 0;
	}
	int ProcessHelp(int index, PCWSTR pszSwitch, PCWSTR pszSwitchUpper)
	{
		return Syntax();
	}
	int ProcessFilespecs()
	{
		// Iterate thru the filespecs
		int nResult = derived().eSuccess;
		for (int i = 1; i < derived().GetArgCount(); ++i)
		{
			wchar_t firstChar = derived().GetArg(i)[0];
			if (firstChar != L'/' && firstChar != L'-')
			{
				// Get the filespec
				auto fileSpec = derived().GetArg(i);

				// Expand wildcards, if specified
				if constexpr (AutoExpand)
				{
					// Split the path information
					wchar_t drive[_MAX_DRIVE * 2];
					wchar_t dir  [_MAX_DIR   * 2];
					_tsplitpath_s(fileSpec.data(),
						drive, _countof(drive),
						dir, _countof(dir),
						nullptr, 0, nullptr, 0);

					// Find the first match, if any
					_tfinddata_t findData = {0};
					intptr_t findHandle = _tfindfirst(fileSpec.data(), &findData);
					if (findHandle == -1)
						return derived().ReportFileError(fileSpec.data());

					// Process matching filename and iterate
					int find;
					do
					{
						// Don't process hidden, system, or directories
						if (!(findData.attrib & (_A_HIDDEN | _A_SYSTEM | _A_SUBDIR)))
						{
							// Create the full filepath
							TCHAR szFullPath[_MAX_PATH * 2];
							_tmakepath_s(szFullPath, drive, dir, findData.name, nullptr);

							// Process the file
							nResult = derived().ProcessFilespec(i, szFullPath);
							if (nResult)
								break;
						}

						// Get the next matching filespec
						find = _tfindnext(findHandle, &findData);

					} while (find != -1);

					// Close the file find handler
					_findclose(findHandle);
				}
				else
				{
					// Process the filespec without expanding it
					nResult = derived().ProcessFilespec(i, fileSpec);
				}
			}
		}

		// Return the last result
		return nResult;
	}
	int ProcessFilespec(int index, std::wstring_view const& fileSpec)
	{
		_ftprintf(derived().GetFileOut(), L"%d) %s\n", index, fileSpec.data());
		return derived().eSuccess;
	}

private:
	inline Derived& derived() { return *static_cast<Derived*>(this); }

// Data Members
protected:
	FILE*  m_fileOut{};
	FILE*  m_fileErr{};
	int    m_argc{};
	wchar_t** m_argv{};
};

