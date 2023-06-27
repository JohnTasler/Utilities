#include "pch.h"
#include "MessageDialogExceptionViewModel.h"
#if __has_include("MessageDialogExceptionViewModel.g.cpp")
#include "MessageDialogExceptionViewModel.g.cpp"
#endif

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::MessageDialogTester::implementation
{
	MessageDialogExceptionViewModel::MessageDialogExceptionViewModel(hresult_error const& error)
	{
		wchar_t buffer[32];
		swprintf_s(buffer, L"0x%08X", static_cast<uint32_t>(error.code()));
		m_propertyHResultFormatted = buffer;
		m_propertyMessage = error.message();
	}
}
