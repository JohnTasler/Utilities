#pragma once

#include "resource.h"

LRESULT CALLBACK
LowLevelMouseProc(
  __in  int    nCode,
  __in  WPARAM wParam,
  __in  LPARAM lParam);

HWND
DeepestChildWindowFromPoint(
	__in  HWND hwndParent,
	__in  POINT point);


HHOOK g_lowLevelMouseHook = nullptr;
