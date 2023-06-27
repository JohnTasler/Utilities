#pragma once

#include "MessageDialogExceptionViewModel.g.h"

#include <mvvm/property_macros.h>
#include <mvvm/view_model_base.h>

namespace winrt::MessageDialogTester::implementation
{
	struct MessageDialogExceptionViewModel
		: MessageDialogExceptionViewModelT<MessageDialogExceptionViewModel>
		, mvvm::view_model_base<MessageDialogExceptionViewModel>
	{
		MessageDialogExceptionViewModel(hresult_error const& error);

		DEFINE_PROPERTY_READONLY(hstring, HResultFormatted, {});
		DEFINE_PROPERTY_READONLY(hstring, Message, {});
	};
}
