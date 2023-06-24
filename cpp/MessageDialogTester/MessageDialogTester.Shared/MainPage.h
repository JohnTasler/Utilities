#pragma once

#include "MainPage.g.h"
#include "StringStringPair.h"
#include <mvvm/property_macros.h>
#include <mvvm/view_model_base.h>

namespace winrt::MessageDialogTester::implementation
{
	namespace wfc = winrt::Windows::Foundation::Collections;
	namespace wux = Windows::UI::Xaml;

	struct MainPage : MainPageT<MainPage>, mvvm::view_model_base<MainPage>
	{
		MainPage() = default;
		void InitializeComponent();

		class_type ViewModel() { return *this; }

		DEFINE_PROPERTY(hstring, Title, L"Hello, world!!!")
		DEFINE_PROPERTY_CALLBACK(hstring, MessageContent, {})

		DEFINE_PROPERTY_READONLY(wfc::IVectorView<StringStringPair::class_type>, QuickContentItems, nullptr)
		DEFINE_PROPERTY_CALLBACK(int32_t, SelectedQuickContentIndex, 1)

		DEFINE_PROPERTY_CALLBACK(int32_t, SelectedButtonCountIndex, -1)

		DEFINE_PROPERTY_READONLY(wfc::IObservableVector<hstring>, DefaultButtonItems, nullptr)
		DEFINE_PROPERTY(int32_t, SelectedDefaultButtonIndex, -1)

		DEFINE_PROPERTY_READONLY(wfc::IObservableVector<hstring>, CancelButtonItems, nullptr)
		DEFINE_PROPERTY(int32_t, SelectedCancelButtonIndex, -1)

		DEFINE_PROPERTY_READONLY(wfc::IObservableVector<StringStringPair::class_type>, ButtonLabels, nullptr)

		void MainPage_Loaded(IInspectable sender, wux::RoutedEventArgs e);
		fire_and_forget Button_Click(IInspectable sender, wux::RoutedEventArgs e);

	private:
		int32_t RefreshButtonIndexItems(std::vector<hstring>& visibleItems, wfc::IObservableVector<hstring> const& items, int32_t selectedButtonIndex);

		bool m_isSettingContentFromComboBox = false;
		std::vector<StringStringPair::class_type> m_allButtonLabels;
		std::vector<StringStringPair::class_type> m_visibleButtonLabels;
		std::vector<hstring> m_visibleDefaultButtonItems;
		std::vector<hstring> m_visibleCancelButtonItems;
	};
}

namespace winrt::MessageDialogTester::factory_implementation
{
	struct MainPage : MainPageT<MainPage, implementation::MainPage>
	{
	};
}
