#include "pch.h"
#include "MessageDialogExceptionView.h"
#if __has_include("MessageDialogExceptionView.g.cpp")
#include "MessageDialogExceptionView.g.cpp"
#endif

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::MessageDialogTester::implementation
{
	MessageDialogExceptionView::MessageDialogExceptionView(MessageDialogExceptionViewModel::class_type const& viewModel)
		: m_propertyViewModel(viewModel)
	{
	}
}
