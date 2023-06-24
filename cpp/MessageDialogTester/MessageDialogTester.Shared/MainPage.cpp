#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"

using namespace std::literals;
using namespace winrt;
using namespace winrt::Windows::Foundation::Collections;
using namespace winrt::Windows::UI::Popups;
using namespace winrt::Windows::UI::Xaml;

namespace winrt::MessageDialogTester::implementation
{
	#pragma region Content Text
	constexpr auto c_ShortText = L"Lorem ipsum dolor sit amet, consectetur adipiscing elit."sv;
	constexpr auto c_MediumText =
		L"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis elit in lacinia tempor."
		L"\n\nAliquam viverra mollis odio, sit amet mollis elit luctus in. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus."
		L"\n\nCum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Curabitur lacinia sed massa in vestibulum. Sed ut mattis sem, nec pretium est. Vestibulum feugiat fringilla viverra."sv;
	constexpr auto c_LongText =
		L"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent molestie odio leo, sit amet vehicula erat venenatis scelerisque. Etiam pulvinar, velit id euismod vulputate, odio dui porttitor mauris, eget posuere dolor tortor in velit. Integer bibendum egestas urna, in pellentesque libero suscipit in. In at nibh eget libero auctor venenatis vitae nec nibh. Suspendisse in ligula in nisl scelerisque aliquet non quis velit. Aliquam erat volutpat. Nunc varius augue vitae tincidunt aliquet. Curabitur dictum tristique mollis. Nulla euismod tincidunt metus, vitae pretium felis. Fusce eget leo id enim eleifend pretium vel non dolor. Nam sollicitudin ante vitae feugiat dignissim. Curabitur consequat risus id dui consectetur, blandit dignissim nisi luctus. Nulla rhoncus id justo eu congue. Sed rhoncus mauris dui, facilisis efficitur velit convallis tempus."
		L"\n\nNullam aliquam sit amet felis nec vehicula. Praesent at arcu ante. Cras convallis ac orci vitae vehicula. Donec viverra, risus sed finibus fermentum, nunc mauris rhoncus magna, a rhoncus lorem metus vitae dui. Fusce ultrices vehicula ante eu luctus. Integer laoreet lorem non ante laoreet consequat. Mauris vel blandit mi. Morbi vel velit sed elit cursus volutpat. Pellentesque interdum blandit sem, quis auctor arcu tempus id. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Proin eu lacinia dolor, eu euismod felis. Cras hendrerit leo ac quam dignissim, quis tincidunt libero commodo. Duis rhoncus, felis nec porta sagittis, nulla nisi dapibus velit, sed ullamcorper elit diam a tellus. Vestibulum nec purus dapibus, hendrerit nisi in, fringilla mi. Duis et vulputate ligula. Duis maximus ullamcorper justo, sit amet ultricies ante hendrerit ac."
		L"\n\nPraesent maximus, mi quis porttitor tempus, tortor enim tincidunt arcu, at semper purus dolor nec metus. Proin non hendrerit elit. Ut quis placerat tellus. Sed tincidunt sagittis lectus, at interdum mauris cursus vel. Nam quis nisl leo. Nulla urna nisi, dignissim luctus rutrum id, fermentum id ligula. Etiam vehicula dui sed ante semper, eu aliquam sapien tristique."
		L"\n\nVestibulum aliquam semper lectus, ut pulvinar lectus bibendum sed. Ut urna ligula, pellentesque vitae felis nec, iaculis iaculis felis. In turpis arcu, gravida sed malesuada vel, venenatis nec orci. Sed laoreet tellus sit amet orci luctus malesuada sed quis risus. Mauris eget elit eu magna pharetra lacinia ut id justo. Sed condimentum sem eu sapien viverra, a dictum eros lacinia. In eleifend vestibulum felis. Proin volutpat viverra justo, ornare dapibus leo congue a. Pellentesque commodo augue volutpat, ultrices lectus a, pharetra sapien. Mauris ac arcu molestie, blandit justo et, sagittis magna. Vestibulum consequat est quis nulla semper, vitae dapibus lacus aliquam. Nunc interdum ex vitae lacus vehicula, varius lobortis leo feugiat. Quisque scelerisque nisl risus, ullamcorper placerat tellus rutrum in. Morbi congue non augue ac posuere."
		L"\n\nCurabitur porttitor nunc ut dolor iaculis pellentesque dapibus non justo. In hac habitasse platea dictumst. Suspendisse potenti. Pellentesque cursus lorem nec est tristique, sed pretium sapien imperdiet. Pellentesque volutpat ultrices leo, venenatis venenatis elit cursus ac. Vestibulum congue orci fermentum, luctus magna in, eleifend elit. In hac habitasse platea dictumst. Proin vel hendrerit urna. Maecenas enim est, vehicula ac metus et, venenatis ultrices nunc. Sed eleifend porta nisl sit amet vestibulum. Pellentesque eu leo sed erat ornare pellentesque eu blandit libero. Sed convallis nisi in purus pulvinar, laoreet venenatis quam imperdiet."
		L"\n\nSed feugiat purus et varius volutpat. Pellentesque eu diam lacinia, volutpat dui eget, condimentum ex. Pellentesque vel porttitor odio. Curabitur iaculis quam in sem rutrum, blandit commodo lacus rhoncus. Nulla pellentesque dui ex, id dignissim orci luctus in. Nunc vulputate odio lacus, sed sollicitudin metus aliquam quis. Praesent ullamcorper risus ut nisi varius, sit amet fermentum ex volutpat. Maecenas suscipit lectus et sapien elementum, sed laoreet nisl interdum. Nulla pretium id nibh porta mattis. Aliquam et erat rhoncus, fermentum ligula vitae, accumsan diam. Phasellus eget ante lectus. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Proin purus ipsum, venenatis a tortor non, pulvinar finibus ex. Donec eros lacus, blandit dapibus volutpat semper, laoreet sit amet mi. Vivamus semper elit metus, eu mattis justo rhoncus venenatis. Curabitur et massa facilisis, aliquet velit in, bibendum lorem."
		L"\n\nPellentesque volutpat ornare tortor, a pretium nisl posuere id. Curabitur leo lorem, tristique a volutpat eu, malesuada eu nisl. Etiam vitae orci vitae metus placerat aliquet eget vel nunc. Aliquam et vestibulum nisi. Mauris vel dictum libero. Suspendisse malesuada sapien dolor, et maximus lacus cursus vitae. Suspendisse eget tincidunt turpis, at porttitor tortor. Ut libero nisl, pharetra quis urna non, elementum ultricies dui."
		L"\n\nVivamus facilisis quis diam eget consectetur. Vivamus ut nisl et sapien hendrerit placerat. Ut pretium purus mauris, eget mattis turpis euismod sit amet. Integer ultrices pellentesque maximus. Ut hendrerit, orci at consectetur eleifend, nisl mauris aliquet neque, eu maximus felis mi at libero. Nam ac diam eget nulla iaculis auctor in at orci. Pellentesque varius nibh vitae metus ultricies fermentum."
		L"\n\nVivamus viverra auctor massa. Maecenas feugiat velit a nulla ornare fermentum. Proin mattis dui nec lectus pulvinar, ac lacinia elit venenatis. Phasellus nisi leo, placerat ac mollis sit amet, fermentum at risus. Donec id dui elit. Etiam a augue nibh. Aenean ut augue vel augue tempus bibendum ut et arcu. Aliquam placerat nisi at pulvinar commodo. Nullam interdum odio sit amet leo venenatis porta. Sed a ante iaculis, tempor tellus vel, convallis massa. Quisque rhoncus odio ac commodo sagittis."
		L"\n\nCras eget nisi volutpat, tempus turpis a, luctus lorem. Curabitur porta, nunc non fringilla tempus, odio libero posuere nulla, in condimentum nunc mauris in leo. Sed semper quam at quam maximus porta. Phasellus quis felis quis sapien tincidunt lobortis. In id aliquet nisi, in porttitor dui. Quisque et odio velit. Morbi fermentum tristique enim, ac mattis neque facilisis a."
		L"\n\nLorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce sodales turpis quis elit varius, ac aliquam nisi dictum. Nam finibus metus ut porta feugiat. Aliquam scelerisque quam ut arcu eleifend, nec ullamcorper justo elementum. Nunc sit amet sollicitudin orci, vel facilisis nulla. Maecenas consequat nunc sit amet dignissim imperdiet. Fusce et dolor vitae purus aliquet facilisis id id nibh. Integer dapibus ex massa, eget vehicula lorem luctus vel. Phasellus ac velit ultricies diam auctor interdum vel suscipit dolor. Ut porta commodo ex et posuere."
		L"\n\nAliquam ut nunc ut turpis vehicula posuere. Morbi gravida, tellus sed vestibulum ornare, mauris risus pulvinar libero, eu mollis mi elit quis odio. Aenean venenatis rutrum convallis. Sed orci erat, condimentum in neque tempus, aliquam fringilla metus. Mauris varius feugiat neque vitae scelerisque. Nunc mattis ex ultricies, facilisis elit bibendum, viverra urna. Suspendisse vehicula hendrerit malesuada. Nulla ut eros maximus, pharetra quam varius, congue diam. Suspendisse in augue id nisl placerat sollicitudin venenatis vel sapien. Curabitur rhoncus, purus quis elementum porta, arcu quam finibus sem, at fermentum nibh libero non turpis. Nulla maximus pharetra sapien, non tempor augue tincidunt in. Duis a feugiat elit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae"sv;
	#pragma endregion Content Text

	void MainPage::InitializeComponent()
	{
		std::vector<StringStringPair::class_type> quickContentItems =
		{
			{ L"(custom)", L"" },
			{ L"Short", c_ShortText },
			{ L"Medium", c_MediumText },
			{ L"Long", c_LongText },
			{ L"Numbering, Bopomofo", L"⒈①Ⅻ〇ㄨㄩ" },
			{ L"Unified Han", L"啊阿鼾齄\n丂丄狚狛\n狜狝﨨﨩" },
			{ L"Modifiers, punctuation, private use area(PUA)", L"ˊ ˋ ˙ – \n" },
			{ L"Unicode Extension A Han", L"㐀㐁䶴䶵" },
			{ L"Unicode Extension B Han", L"𠀀𠀁𪛕𪛖" },
			{ L"Tibetan", L"དབྱངས་ཅན་སྒྲོལ་དཀར། བཀྲ་ཤིས་རྒྱལ།" },
			{ L"Traditional Mongolian", L"ᠣᠺᠵᠡᠵᠺᠰ ᠬᠵᠠᠬᠳᠰᠣᠬᠡᠵᠬ ᠬᠵᠺᠠᠬᠳᠰ ᠺᠬᠣ ᠠ" },
			{ L"Yi", L"ꆈꌠꁱꂷꀀꀁꀂꉬꄒꐵꄓꐨ" },
			{ L"Dehong Dai / Tai Le", L"ᥘᥣᥭᥰ ᥖᥭᥰ ᥖᥬᥲᥑᥨᥒᥰ" },
			{ L"Xishuangbanna Dai / New Tai Lue", L"ᦎᦷᦑᦺᦖᦺᧈᦉᦲᧇᦉᦸᧂᦗᧃᦓᦱ" },
		};
		m_propertyQuickContentItems =
			single_threaded_vector(std::move(quickContentItems)).GetView();

		auto makeButtonItemsVector = []
		{
			return std::vector<hstring>
			{
				L"None",
				L"0",
				L"1",
				L"2",
				L"3",
				L"4",
			};
		};

		m_propertyDefaultButtonItems =
			single_threaded_observable_vector<hstring>(std::move(makeButtonItemsVector()));

		m_propertyCancelButtonItems =
			single_threaded_observable_vector<hstring>(std::move(makeButtonItemsVector()));

		m_propertyButtonLabels = single_threaded_observable_vector<StringStringPair::class_type>();

		m_allButtonLabels =
		{
			make<StringStringPair>(L"Button 1 Label", L"Save"),
			make<StringStringPair>(L"Button 2 Label", L"Don't Save"),
			make<StringStringPair>(L"Button 3 Label", L"Cancel"),
		};

		SelectedButtonCountIndex(3);
	}

	void MainPage::OnMessageContentChanged(hstring const& oldValue, hstring const& newValue)
	{
		if (!m_isSettingContentFromComboBox)
		{
			m_propertyQuickContentItems.First().Current().Value(newValue);
			SelectedQuickContentIndex(0);
		}
	}

	void MainPage::OnSelectedQuickContentIndexChanged(int32_t const& oldValue, int32_t const& newValue)
	{
		m_isSettingContentFromComboBox = true;
		MessageContent(m_propertyQuickContentItems.GetAt(static_cast<uint32_t>(newValue)).Value());
		m_isSettingContentFromComboBox = false;
	}

	void MainPage::OnSelectedButtonCountIndexChanged(int32_t const& oldValue, int32_t const& newValue)
	{
		m_propertyButtonLabels.Clear();

		for (auto const& buttonLabel : m_allButtonLabels)
		{
			m_propertyButtonLabels.Append(buttonLabel);
		}

		SelectedDefaultButtonIndex(RefreshButtonIndexItems(m_visibleDefaultButtonItems, m_propertyDefaultButtonItems, m_propertySelectedDefaultButtonIndex));
		SelectedCancelButtonIndex(RefreshButtonIndexItems(m_visibleCancelButtonItems, m_propertyCancelButtonItems, m_propertySelectedCancelButtonIndex));
	}

	int32_t MainPage::RefreshButtonIndexItems(std::vector<hstring>& visibleItems, IObservableVector<hstring> const& items, int32_t selectedButtonIndex)
	{
		visibleItems.clear();

		for (auto const& item : items)
		{
			visibleItems.push_back(item);
		}

		return std::min(selectedButtonIndex, m_propertySelectedButtonCountIndex);
	}

	void MainPage::MainPage_Loaded(IInspectable sender, RoutedEventArgs e)
	{
		SelectedButtonCountIndex(3);
		SelectedDefaultButtonIndex(1);
		SelectedCancelButtonIndex(3);
	}

	fire_and_forget MainPage::Button_Click(IInspectable sender, RoutedEventArgs e)
	{
		try
		{
			MessageDialog dialog{ MessageContent(), Title() };
			dialog.DefaultCommandIndex(m_propertySelectedDefaultButtonIndex - 1);
			dialog.CancelCommandIndex(m_propertySelectedCancelButtonIndex - 1);

			auto commands = dialog.Commands();
			commands.Clear();
			for (auto label : m_visibleButtonLabels)
			{
				commands.Append(UICommand{ label.Value() });
			}

			co_await dialog.ShowAsync();
		}
		catch (hresult_error const& ex)
		{
			OutputDebugStringW(ex.message().c_str());

			// TODO: Create result view models and show ContentDialog

		}
	}
}
