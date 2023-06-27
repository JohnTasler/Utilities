#include "pch.h"
#include "MessageDialogResultView.h"

using namespace winrt;

namespace winrt::MessageDialogTester::implementation
{
	MessageDialogResultView::MessageDialogResultView(MessageDialogResultViewModel::class_type const& viewModel)
		: m_propertyViewModel(viewModel)
	{
	}
}
