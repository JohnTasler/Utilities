#pragma once

// Windows headers
// Exclude rarely-used stuff from Windows headers
#define WIN32_LEAN_AND_MEAN
#include <windows.h>

// Standard C headers
#include <errno.h>
#include <io.h>
#include <malloc.h>
#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>

// Standard C/C++ headers
#include <chrono>
#include <experimental/generator>
#include <filesystem>
#include <string>
#include <string_view>
#include <system_error>
#include <thread>
#include <vector>

using namespace std::literals;
