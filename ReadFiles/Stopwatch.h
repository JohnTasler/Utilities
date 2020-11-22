/////////////////////////////////////////////////////////////////////////////
// Stopwatch.h
//

#pragma once
#include <windows.h>


/////////////////////////////////////////////////////////////////////////////
//
class CStopwatch
{
// Constants
private:
	static const std::int64_t TicksPerMillisecond = 0x2710L;
	static const LONGLONG TicksPerSecond = 0x989680L;

// Fields
private:
	LONGLONG elapsed;
	LONGLONG startTimeStamp;
	bool isRunning;
	static double tickFrequency;

// Constructor
public:
	CStopwatch()
	{
		CStopwatch::InitializeType();
		this->Reset();
	}

// Public Fields
public:
	static LONGLONG Frequency;
	static bool IsHighResolution;

// Properties
public:
	LONGLONG get_Elapsed()
	{
		return this->GetElapsedDateTimeTicks();
	}

	LONGLONG get_ElapsedMilliseconds()
	{
		return this->GetElapsedDateTimeTicks() / 0x2710L;
	}

	LONGLONG get_ElapsedTicks()
	{
		return this->GetRawElapsedTicks();
	}

	bool get_IsRunning()
	{
		return this->isRunning;
	}

// Methods
public:
	static LONGLONG GetTimestamp()
	{
		if (IsHighResolution)
		{
			LARGE_INTEGER num = { 0 };
			QueryPerformanceCounter(&num);
			return num.QuadPart;
		}

		FILETIME ft;
		GetSystemTimeAsFileTime(&ft);
		ULARGE_INTEGER ftUint = { ft.dwLowDateTime, ft.dwHighDateTime };
		return ftUint.QuadPart + 0x701ce1722770000L;
	}

	void Reset()
	{
		this->elapsed = 0L;
		this->isRunning = false;
		this->startTimeStamp = 0L;
	}

	void Restart()
	{
		this->elapsed = 0L;
		this->startTimeStamp = CStopwatch::GetTimestamp();
		this->isRunning = true;
	}

	void Start()
	{
		if (!this->isRunning)
		{
			this->startTimeStamp = CStopwatch::GetTimestamp();
			this->isRunning = true;
		}
	}

	void Stop()
	{
		if (this->isRunning)
		{
			LONGLONG num2 = CStopwatch::GetTimestamp() - this->startTimeStamp;
			this->elapsed += num2;
			this->isRunning = false;
			if (this->elapsed < 0L)
				this->elapsed = 0L;
		}
	}

// Private implementation
private:
	static void InitializeType()
	{
		static volatile LONG isInitialized = 0;
		if (!isInitialized)
		{
			bool isHighResolution;
			LARGE_INTEGER frequency;
			double tickFrequency;

			if (!QueryPerformanceFrequency(&frequency))
			{
				isHighResolution = false;
				frequency.QuadPart = 0x989680L;
				tickFrequency = 1.0;
			}
			else
			{
				isHighResolution = true;
				tickFrequency = 10000000.0;
				tickFrequency /= (double)frequency.QuadPart;
			}

			if (!isInitialized)
			{
				CStopwatch::IsHighResolution = isHighResolution;
				CStopwatch::Frequency = frequency.QuadPart;
				CStopwatch::tickFrequency = tickFrequency;
				InterlockedExchange(&isInitialized, 1);
			}
		}
	}

	LONGLONG GetElapsedDateTimeTicks()
	{
		LONGLONG rawElapsedTicks = this->GetRawElapsedTicks();
		if (CStopwatch::IsHighResolution)
		{
			double num2 = (double)rawElapsedTicks;
			num2 *= CStopwatch::tickFrequency;
			return (LONGLONG)num2;
		}
		return rawElapsedTicks;
	}

	LONGLONG GetRawElapsedTicks()
	{
		LONGLONG elapsed = this->elapsed;
		if (this->isRunning)
		{
			LONGLONG num3 = CStopwatch::GetTimestamp() - this->startTimeStamp;
			elapsed += num3;
		}
		return elapsed;
	}
};

double CStopwatch::tickFrequency;
LONGLONG CStopwatch::Frequency;
bool CStopwatch::IsHighResolution;
