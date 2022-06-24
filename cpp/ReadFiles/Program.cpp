/////////////////////////////////////////////////////////////////////////////
// Program.cpp : Defines the entry point for the console application.
//

#include "pch.h"
#include "file_processor.h"
#include "syntax.h"

/////////////////////////////////////////////////////////////////////////////
// read_files_app
class read_files_app
	: public file_processor<read_files_app, true>
{
// Constants
private:
	inline static constexpr int c_cbPage = 4096;
	inline static constexpr int c_defaultPageCount = 512;
	inline static constexpr int c_defaultThreadCount = 1;

// Data Members
private:
	size_t m_pageCount{ c_defaultPageCount };
	size_t m_maxThreadCount{ std::thread::hardware_concurrency() };
	size_t m_threadCount{ m_maxThreadCount };
	bool m_isPageCountSpecified{};
	bool m_isThreadCountSpecified{};
	std::vector<void*> m_buffers;

	void* m_buffer{};
	size_t m_bufferSectionByteCount{};

	std::wstring_view m_fileSpec;
	HANDLE m_hFile{};
	int64_t volatile m_bytesRead{};
	int64_t volatile m_nextFileOffset{};
	int64_t volatile m_nextBufferOffset{};

// Construction / Destruction
public:
	read_files_app() = default;

// Overrides
public:
	std::wstring_view GetSyntax()
	{
		return c_syntaxText.substr(1);
	}

	int GetSwitchSizeValue(std::wstring_view const& switchTest, std::wstring_view const& switchString, std::wstring_view const& switchUpper,
		size_t* pnValue, bool* pbConverted, bool* pbIsSwitch)
	{
		if (pbConverted)
		{
			*pbConverted = false;
		}
		if (pbIsSwitch)
		{
			*pbIsSwitch = false;
		}

		if (switchTest == switchString.substr(0, switchTest.length())
		||  switchTest == switchUpper.substr(0, switchTest.length()))
		{
			if (pbIsSwitch)
			{
				*pbIsSwitch = true;
			}

			if constexpr (sizeof(size_t) == sizeof(uint64_t))
			{
				*pnValue = static_cast<size_t>(std::wcstoll(switchString.data() + switchTest.length(), nullptr, 0));
			}
			else
			{
				*pnValue = static_cast<size_t>(std::wcstol(switchString.data() + switchTest.length(), nullptr, 0));
			}

			if ( (static_cast<int64_t>(*pnValue) == _I64_MAX) || ( (*pnValue == 0) && errno == ERANGE ) )
				return Syntax(L"Invalid switch format or value: /%ls\n"sv, switchString.data());

			if (pbConverted)
			{
				*pbConverted = true;
			}
		}

		return eSuccess;
	}

	int ProcessSwitch(int index, std::wstring_view const& pszSwitch, std::wstring_view const& pszSwitchUpper)
	{
		// Perform default processing (to check for standard /? /HELP switches)
		int nResult = file_processor_base::ProcessSwitch(index, pszSwitch, pszSwitchUpper);
		if (nResult != 0)
			return nResult;

		// Check for our specific switches
		bool isConverted;
		bool isSwitch;

		nResult = GetSwitchSizeValue(L"P:"sv, pszSwitch, pszSwitchUpper, &m_pageCount, &isConverted, &isSwitch);
		if (nResult != 0)
			return nResult;
		if (isSwitch)
		{
			if (m_isPageCountSpecified)
				return Syntax(L"Switch can only be specified once: /P\n");

			if (0 >= m_pageCount || m_pageCount > 4096)
				return Syntax(L"page_count must be between 1 and 4096, inclusive.\n");

			m_isPageCountSpecified = true;
			return eSuccess;
		}

		if (auto nResult = GetSwitchSizeValue(L"T:"sv, pszSwitch, pszSwitchUpper, &m_threadCount, &isConverted, &isSwitch))
			return nResult;

		if (isSwitch)
		{
			if (m_isThreadCountSpecified)
				return Syntax(L"Switch can only be specified once: /T\n");

			if (0 >= m_threadCount || m_threadCount > m_maxThreadCount)
				return Syntax(L"thread_count must be between 1 and %u, inclusive.\n", m_maxThreadCount);

			m_isPageCountSpecified = true;
			return eSuccess;
		}

		// Switch is unrecognized
		return Syntax(L"Invalid switch: /%ls\n", pszSwitch);
	}

	int ProcessFileSpecs()
	{
		m_bufferSectionByteCount = c_cbPage * m_pageCount;
		auto bufferEntireByteCount = m_bufferSectionByteCount * m_threadCount;
		m_buffer = VirtualAlloc(nullptr, bufferEntireByteCount, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE | PAGE_NOCACHE);

		if (m_buffer == nullptr)
		{
			DWORD dwLastError = GetLastError();
			fwprintf(this->GetFileErr(),
				L"Error %08X allocating buffer of size %Iu\n",
				dwLastError, bufferEntireByteCount);
			return (int)dwLastError;
		}

		fwprintf(this->GetFileOut(), L"Allocated buffer of size %Iu\n", bufferEntireByteCount);

		auto startTimePoint = std::chrono::high_resolution_clock::now();
		int result = file_processor_base::ProcessFileSpecs();
		auto elapsedTime = std::chrono::high_resolution_clock::now() - startTimePoint;
		auto elapsedSeconds = static_cast<double>(std::chrono::duration_cast<std::chrono::seconds>(elapsedTime).count());

		fwprintf(this->GetFileOut(), L"%I64d bytes were read in %.4f seconds.\n",
			m_bytesRead, elapsedSeconds);

		fwprintf(this->GetFileOut(), L"    %.2f MiB/Second\n",
			(double(m_bytesRead) / (1024 * 1024)) / elapsedSeconds
			);

		fwprintf(this->GetFileOut(), L"    %.2f Mibps\n",
			((double(m_bytesRead) * 8) / (1024 * 1024)) / elapsedSeconds
			);

		return result;
	}

	int ProcessFileSpec(int index, std::wstring_view const& fileSpec)
	{
		// Save the file spec
		m_fileSpec = fileSpec;

		// Open the file with no caching
		m_hFile = CreateFile(
			fileSpec.data(), FILE_READ_DATA, FILE_SHARE_READ, nullptr, OPEN_EXISTING,
			FILE_ATTRIBUTE_NORMAL | FILE_FLAG_NO_BUFFERING | FILE_FLAG_OVERLAPPED, nullptr);

		if (m_hFile != INVALID_HANDLE_VALUE)
		{
			LARGE_INTEGER fileSize = { 0 };
			if (!GetFileSizeEx(m_hFile, &fileSize))
			{
				DWORD dwLastError = GetLastError();
				fwprintf(this->GetFileErr(), L"Error %08X getting size of file \"%ls\"\n", dwLastError, fileSpec.data());
				return eSuccess;
			}

			fwprintf(this->GetFileOut(), L"Reading %I64d bytes from file \"%ls\"...", fileSize.QuadPart, fileSpec.data());

			// Start negative so each thread doesn't have to subtract
			m_nextFileOffset -= m_bufferSectionByteCount;
			m_nextBufferOffset -= m_bufferSectionByteCount;

			// Create and start each thread
			std::vector<HANDLE> threadHandles(m_threadCount);
			for (size_t threadIndex = 0; threadIndex < m_threadCount; ++threadIndex)
			{
				threadHandles[threadIndex] = CreateThread(nullptr, 0, &read_files_app::thread_proc_thunk, this, 0, nullptr);
			}

			// Wait for all of the threads to complete
			WaitForMultipleObjects(static_cast<DWORD>(m_threadCount), threadHandles.data(), true, INFINITE);

			fwprintf(this->GetFileOut(), L"done\n");
		}
		else
		{
			DWORD dwLastError = GetLastError();
			fwprintf(this->GetFileErr(), L"Error 0x%08X opening file \"%s\"\n", dwLastError, fileSpec.data());
			return eFileError;
		}

		// Indicate success
		return eSuccess;
	}

	static DWORD WINAPI thread_proc_thunk(void* context)
	{
		reinterpret_cast<read_files_app*>(context)->thread_proc();
		return 0;
	}

	void thread_proc()
	{
		auto bufferSectionByteCount = m_bufferSectionByteCount;
		auto bufferSection = static_cast<std::uint8_t*>(m_buffer) + InterlockedAdd64(&m_nextBufferOffset, bufferSectionByteCount);

		HANDLE hEvent = CreateEvent(nullptr, true, false, nullptr);

		for (DWORD cbRead = 1; cbRead != 0; bufferSection)
		{
			OVERLAPPED overlapped = { 0 };
			int64_t myFileOffset = InterlockedAdd64(&m_nextFileOffset, bufferSectionByteCount);
			overlapped.Offset = static_cast<DWORD>(myFileOffset & 0x00000000FFFFFFFF);
			overlapped.OffsetHigh = static_cast<DWORD>(myFileOffset >> 32);
			overlapped.hEvent = hEvent;

			uint32_t dwLastError = 0;
			if (!ReadFile(m_hFile, bufferSection, static_cast<DWORD>(bufferSectionByteCount), nullptr, &overlapped))
			{
				dwLastError = GetLastError();
				if (dwLastError != ERROR_IO_PENDING &&
					dwLastError != ERROR_HANDLE_EOF &&
					dwLastError != ERROR_NOT_DOS_DISK)
				{
					fwprintf(this->GetFileOut(), L"error\n");
					fwprintf(this->GetFileErr(), L"Error %08X reading file \"%ls\"\n", dwLastError, m_fileSpec.data());
					goto cleanup;
				}
			}

			cbRead = 0;
			if (dwLastError != ERROR_HANDLE_EOF && !GetOverlappedResult(m_hFile, &overlapped, &cbRead, true))
			{
				uint32_t dwLastError = GetLastError();
				if (dwLastError != ERROR_HANDLE_EOF)
				{
					fwprintf(this->GetFileOut(), L"error\n");
					fwprintf(this->GetFileErr(), L"Error %08X getting overlapped result for file \"%ls\"\n", dwLastError, m_fileSpec.data());
					goto cleanup;
				}
				else
				{
					cbRead = 0;
				}
			}

			InterlockedAdd64(&m_bytesRead, cbRead);
		}

	cleanup:
		(void)CloseHandle(hEvent);
	}
};

/////////////////////////////////////////////////////////////////////////////
//
int wmain(int argc, wchar_t* argv[])
{
	read_files_app app;
	int nResult = app.Main(argc, argv);
	return nResult;
}
