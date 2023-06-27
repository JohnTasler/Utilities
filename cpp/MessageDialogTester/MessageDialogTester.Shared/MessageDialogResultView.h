#pragma once

//#include "winrt/Windows.UI.Popups.h"
//#include "winrt/Windows.UI.Xaml.h"
//#include "winrt/Windows.UI.Xaml.Markup.h"
//#include "winrt/Windows.UI.Xaml.Interop.h"
//#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "MessageDialogResultView.g.h"
#include "MessageDialogResultViewModel.h"

#include <mvvm/property_macros.h>
#include <mvvm/view_model_base.h>

namespace winrt::MessageDialogTester::implementation
{
	struct MessageDialogResultView
		: MessageDialogResultViewT<MessageDialogResultView>
		, mvvm::view_model_base<MessageDialogResultView>
	{
		MessageDialogResultView(MessageDialogResultViewModel::class_type const& viewModel);

		DEFINE_PROPERTY_READONLY(MessageDialogResultViewModel::class_type, ViewModel, nullptr);
	};
}
