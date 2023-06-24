//
// pch.h
// Header for standard system include files.
//

#pragma once

#include <algorithm>
#include <string>

#define NOMINMAX
template <class T> const T& min(const T& a, const T& b) { return std::min(a, b); }
template <class T> const T& max(const T& a, const T& b) { return std::max(a, b); }

#include <collection.h>
#include <ppltasks.h>

