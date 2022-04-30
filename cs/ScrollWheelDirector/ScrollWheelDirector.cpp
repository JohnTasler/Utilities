// ScrollWheelDirector.cpp : Defines the entry point for the application.
//

#include "pch.h"
#include "ScrollWheelDirector.h"

#define MAX_LOADSTRING 100

int APIENTRY
_tWinMain(
	_In_     HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_     LPTSTR    lpCmdLine,
	_In_     int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	// Create the message loop
	MSG msg;
	PeekMessage(&msg, NULL, WM_USER, WM_USER, PM_NOREMOVE);

	// TODO: Determine if another instance of the application is already running
	// TODO: If so, either just exit if no cmdline args, or EXIT commandline arg, then post a quit message to the thread of the first instance (create an event is probably best, but it will require changing the GetMessage Call to MsgWaitForMultipleObjects)
	// TODO: Another option would be to display a dialog to allow you to control/configure this message loop.

	// Register the low-level mouse hook procedure
//	auto threadId = GetCurrentThreadId();
	g_lowLevelMouseHook = SetWindowsHookEx(WH_MOUSE_LL, LowLevelMouseProc, NULL, 0);

	// Main message loop:
	while (GetMessage(&msg, NULL, 0, 0))
		DispatchMessage(&msg);

	UnhookWindowsHookEx(g_lowLevelMouseHook);
	g_lowLevelMouseHook = nullptr;

	return static_cast<int>(msg.wParam);
}


LRESULT CALLBACK
LowLevelMouseProc(
  __in  int    nCode,
  __in  WPARAM wParam,
  __in  LPARAM lParam)
{
	if (nCode >= 0 && (wParam == WM_MOUSEWHEEL || wParam == WM_MOUSEHWHEEL))
	{
		auto message = reinterpret_cast<MSLLHOOKSTRUCT&>(lParam);
		auto point = message.pt;

		auto hwnd = WindowFromPoint(point);
		if (hwnd != nullptr)
		{
			ScreenToClient(hwnd, &point);
			hwnd = DeepestChildWindowFromPoint(hwnd, point);

			GUITHREADINFO threadInfo = { sizeof(threadInfo) };
			GetGUIThreadInfo(NULL, &threadInfo);
			if (!(threadInfo.flags & GUI_INMOVESIZE) && threadInfo.hwndCapture == nullptr && threadInfo.hwndFocus != hwnd)
			{
				point = message.pt;
				ScreenToClient(hwnd, &point);

				auto msg    = MSG();
				msg.time    = message.time;
				msg.pt      = point;
				msg.hwnd    = hwnd;
				msg.message = wParam;
				msg.wParam  = message.mouseData;  // TODO: Do we need to create the flags for the virtual keys?
				msg.lParam  = MAKELPARAM(message.pt.x, message.pt.y);

				auto result = SendMessage(msg.hwnd, msg.message, msg.wParam, msg.lParam);
				if (result == 0)
					return true;
			}
		}
	}

	return CallNextHookEx(g_lowLevelMouseHook, nCode, wParam, lParam);
}

HWND DeepestChildWindowFromPoint(HWND hwndParent, POINT point)
{
	auto hwnd = ChildWindowFromPointEx(hwndParent, point,
		CWP_SKIPDISABLED | CWP_SKIPINVISIBLE | CWP_SKIPTRANSPARENT);

	if (hwnd == nullptr || hwnd == hwndParent)
		return hwndParent;

	MapWindowPoints(hwndParent, hwnd, &point, 1);
	return DeepestChildWindowFromPoint(hwnd, point);
}


