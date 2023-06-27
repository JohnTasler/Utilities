#pragma once

#include "winrt/Windows.UI.Popups.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "MessageDialogExceptionView.g.h"
#include "MessageDialogExceptionViewModel.h"

#include <mvvm/property_macros.h>
#include <mvvm/view_model_base.h>

namespace winrt::MessageDialogTester::implementation
{
	struct MessageDialogExceptionView
		: MessageDialogExceptionViewT<MessageDialogExceptionView>
		, mvvm::view_model_base<MessageDialogExceptionView>
	{
		MessageDialogExceptionView(MessageDialogExceptionViewModel::class_type const& viewModel);

		DEFINE_PROPERTY_READONLY(MessageDialogExceptionViewModel::class_type, ViewModel, nullptr);
	};
}
