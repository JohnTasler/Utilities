/////////////////////////////////////////////////////////////////////////////
// Program.cpp : Defines the entry point for the console application.
//

#include "pch.h"
#include "FileProcessor.h"
#include "Stopwatch.h"
#include "Syntax.h"


/////////////////////////////////////////////////////////////////////////////
// CReadFilesApp
class CReadFilesApp
	: public CFileProcessor<CReadFilesApp, false>
{
// Constants
private:
	static constexpr int c_cbPage = 4096;
	static constexpr int c_defaultPageCount = 512;
	static constexpr int c_defaultThreadCount = 1;

// Data Members
private:
	size_t m_pageCount = c_defaultPageCount;
	size_t m_threadCount = c_defaultThreadCount;
	size_t m_maxThreadCount = c_defaultThreadCount;
	bool m_isPageCountSpecified = false;
	bool m_isThreadCountSpecified = false;
	CStopwatch m_stopwatch;
	std::vector<void*> m_buffers;

	void* m_buffer = nullptr;
	size_t m_bufferSectionByteCount = 0;

	PCWSTR m_fileSpec = nullptr;
	HANDLE m_hFile = nullptr;
	int64_t volatile m_bytesRead = 0;
	int64_t volatile m_nextFileOffset = 0;
	int64_t volatile m_nextBufferOffset = 0;

// Construction / Destruction
public:
	CReadFilesApp()
		: m_threadCount(m_maxThreadCount = GetMaxNumberOfThreads())
	{
	}

// Overrides
public:
	PCWSTR GetSyntax()
	{
		return c_syntaxText + 1;
	}

	int GetSwitchValue(PCWSTR pszSwitchTest, PCWSTR pszSwitch, PCWSTR pszSwitchUpper,
		size_t* pnValue, bool* pbConverted, bool* pbIsSwitch)
	{
		*pbConverted = false;
		*pbIsSwitch = false;

		size_t cchSwitchTest = _tcslen(pszSwitchTest);

		if (0 == _tcsncmp(pszSwitchUpper, pszSwitchTest, cchSwitchTest))
		{
			*pbIsSwitch = true;
			*pnValue = _tcstol(pszSwitch + cchSwitchTest, NULL, 0);

			if ( (*pnValue == _I64_MAX) || ( (*pnValue == 0) && errno == ERANGE ) )
				return Syntax(L"Invalid switch format or value: /%s\n", pszSwitch);

			*pbConverted = true;
		}

		return eSuccess;
	}

	int ProcessSwitch(int index, PCWSTR pszSwitch, PCWSTR pszSwitchUpper)
	{
		// Perform default processing (to check for standard /? /HELP switches)
		int nResult = CFileProcessorBase::ProcessSwitch(index, pszSwitch, pszSwitchUpper);
		if (nResult != 0)
			return nResult;

		// Check for our specific switches
		bool isConverted;
		bool isSwitch;

		nResult = GetSwitchValue(L"P:", pszSwitch, pszSwitchUpper, &m_pageCount, &isConverted, &isSwitch);
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

		if (auto nResult = GetSwitchValue(L"T:", pszSwitch, pszSwitchUpper, &m_threadCount, &isConverted, &isSwitch))
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
		return Syntax(L"Invalid switch: /%s\n", pszSwitch);
	}

	int ProcessFilespecs()
	{
		m_bufferSectionByteCount = c_cbPage * m_pageCount;
		auto bufferEntireByteCount = m_bufferSectionByteCount * m_threadCount;
		m_buffer = VirtualAlloc(NULL, bufferEntireByteCount, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE | PAGE_NOCACHE);

		if (m_buffer == NULL)
		{
			DWORD dwLastError = GetLastError();
			_ftprintf(this->GetFileErr(),
				L"Error %08X allocating buffer of size %Iu\n",
				dwLastError, bufferEntireByteCount);
			return (int)dwLastError;
		}

		_ftprintf(this->GetFileOut(), L"Allocated buffer of size %Iu\n", bufferEntireByteCount);

		int result = CFileProcessorBase::ProcessFilespecs();

		_ftprintf(this->GetFileOut(), L"%I64d bytes were read in %.4f seconds.\n",
			m_bytesRead, m_stopwatch.get_ElapsedMilliseconds() / 1000.0f);

		_ftprintf(this->GetFileOut(), L"    %.2f MB/Second\n",
			(double(m_bytesRead) / (1024 * 1024)) / (m_stopwatch.get_ElapsedMilliseconds() / 1000.0f)
			);

		_ftprintf(this->GetFileOut(), L"    %.2f Mbps\n",
			((double(m_bytesRead) * 8) / (1024 * 1024)) / (m_stopwatch.get_ElapsedMilliseconds() / 1000.0f)
			);

		return result;
	}

	int ProcessFilespec(int index, PCWSTR pszFilespec)
	{
		// Save the file spec
		m_fileSpec = pszFilespec;

		// Open the file with no caching
		m_hFile = CreateFile(
			pszFilespec, FILE_READ_DATA, FILE_SHARE_READ, NULL, OPEN_EXISTING,
			FILE_ATTRIBUTE_NORMAL | FILE_FLAG_NO_BUFFERING | FILE_FLAG_OVERLAPPED, NULL);

		if (m_hFile != INVALID_HANDLE_VALUE)
		{
			LARGE_INTEGER fileSize = { 0 };
			if (!GetFileSizeEx(m_hFile, &fileSize))
			{
				DWORD dwLastError = GetLastError();
				_ftprintf(this->GetFileErr(), L"Error %08X getting size of file \"%s\"\n", dwLastError, pszFilespec);
				return eSuccess;
			}

			_ftprintf(this->GetFileOut(), L"Reading %I64d bytes from file \"%s\"...", fileSize.QuadPart, pszFilespec);

			// Start negative so each thread doesn't have to subtract
			m_nextFileOffset -= m_bufferSectionByteCount;
			m_nextBufferOffset -= m_bufferSectionByteCount;

			// Read the entire file
			m_stopwatch.Start();

			// Create and start each thread
			std::vector<HANDLE> threadHandles(m_threadCount);
			for (size_t threadIndex = 0; threadIndex < m_threadCount; ++threadIndex)
			{
				threadHandles[threadIndex] = CreateThread(NULL, 0, &CReadFilesApp::ThreadProcThunk, this, 0, nullptr);
			}

			// Wait for all of the threads to complete
			WaitForMultipleObjects(static_cast<DWORD>(m_threadCount), threadHandles.data(), true, INFINITE);

			if (m_stopwatch.get_IsRunning())
			{
				m_stopwatch.Stop();
				_ftprintf(this->GetFileOut(), L"done\n");
			}
		}
		else
		{
			DWORD dwLastError = GetLastError();
			_ftprintf(this->GetFileErr(), L"Error 0x%08X opening file \"%s\"\n", dwLastError, pszFilespec);
			return eFileError;
		}

		// Indicate success
		return eSuccess;
	}

	static DWORD GetMaxNumberOfThreads()
	{
		SYSTEM_INFO sysInfo;
		GetSystemInfo(&sysInfo);
		return sysInfo.dwNumberOfProcessors;
	}

	static DWORD WINAPI ThreadProcThunk(void* context)
	{
		reinterpret_cast<CReadFilesApp*>(context)->ThreadProc();
		return 0;
	}

	void ThreadProc()
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
			if (!ReadFile(m_hFile, bufferSection, static_cast<DWORD>(bufferSectionByteCount), NULL, &overlapped))
			{
				dwLastError = GetLastError();
				if (dwLastError != ERROR_IO_PENDING &&
					dwLastError != ERROR_HANDLE_EOF &&
					dwLastError != ERROR_NOT_DOS_DISK)
				{
					_ftprintf(this->GetFileOut(), L"error\n");
					_ftprintf(this->GetFileErr(), L"Error %08X reading file \"%s\"\n", dwLastError, m_fileSpec);
					goto cleanup;
				}
			}

			cbRead = 0;
			if (dwLastError != ERROR_HANDLE_EOF && !GetOverlappedResult(m_hFile, &overlapped, &cbRead, true))
			{
				uint32_t dwLastError = GetLastError();
				if (dwLastError != ERROR_HANDLE_EOF)
				{
					_ftprintf(this->GetFileOut(), L"error\n");
					_ftprintf(this->GetFileErr(), L"Error %08X getting overlapped result for file \"%s\"\n", dwLastError, m_fileSpec);
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
	CReadFilesApp app;
	int nResult = app.Main(argc, argv);
	return nResult;
}
