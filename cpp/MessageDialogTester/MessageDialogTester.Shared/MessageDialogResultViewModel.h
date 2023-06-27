#pragma once

#include "MessageDialogResultViewModel.g.h"
#include <mvvm/property_macros.h>
#include <mvvm/view_model_base.h>

namespace winrt::MessageDialogTester::implementation
{
	struct MessageDialogResultViewModel
		: MessageDialogResultViewModelT<MessageDialogResultViewModel>
		, mvvm::view_model_base<MessageDialogResultViewModel>
	{
		MessageDialogResultViewModel(Windows::UI::Popups::IUICommand const& command, uint32_t index);

		DEFINE_PROPERTY_READONLY(Windows::UI::Popups::IUICommand, Command, {});
		DEFINE_PROPERTY_READONLY(uint32_t, Index, {});
	};
}
