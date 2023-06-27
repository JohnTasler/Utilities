#include "pch.h"
#include "MessageDialogResultViewModel.h"

using namespace winrt::Windows::UI::Popups;

namespace winrt::MessageDialogTester::implementation
{
	MessageDialogResultViewModel::MessageDialogResultViewModel(IUICommand const& command, uint32_t index)
		: m_propertyCommand(command)
		, m_propertyIndex(index)
	{
	}
}
