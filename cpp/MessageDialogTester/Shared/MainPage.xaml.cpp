//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "MessageDialogResult.xaml.h"
#include "MessageDialogException.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Popups;
using namespace MessageDialogTester;

constexpr PCWSTR c_ShortText = L"Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
constexpr PCWSTR c_MediumText =
    L"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis elit in lacinia tempor."
    L"\n\nAliquam viverra mollis odio, sit amet mollis elit luctus in. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus."
    L"\n\nCum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Curabitur lacinia sed massa in vestibulum. Sed ut mattis sem, nec pretium est. Vestibulum feugiat fringilla viverra.";
constexpr PCWSTR c_LongText =
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
    L"\n\nAliquam ut nunc ut turpis vehicula posuere. Morbi gravida, tellus sed vestibulum ornare, mauris risus pulvinar libero, eu mollis mi elit quis odio. Aenean venenatis rutrum convallis. Sed orci erat, condimentum in neque tempus, aliquam fringilla metus. Mauris varius feugiat neque vitae scelerisque. Nunc mattis ex ultricies, facilisis elit bibendum, viverra urna. Suspendisse vehicula hendrerit malesuada. Nulla ut eros maximus, pharetra quam varius, congue diam. Suspendisse in augue id nisl placerat sollicitudin venenatis vel sapien. Curabitur rhoncus, purus quis elementum porta, arcu quam finibus sem, at fermentum nibh libero non turpis. Nulla maximus pharetra sapien, non tempor augue tincidunt in. Duis a feugiat elit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae";

MainPage::MainPage()
{
    m_title = ref new String(L"Hello, world!!!");

    m_quickContentItems = ref new Vector<StringStringPair^>();
    m_quickContentItems->Append(ref new StringStringPair(ref new String(L"(custom)"), nullptr));
    m_quickContentItems->Append(ref new StringStringPair(ref new String(L"Short"), ref new String(c_ShortText)));
    m_quickContentItems->Append(ref new StringStringPair(ref new String(L"Medium"), ref new String(c_MediumText)));
    m_quickContentItems->Append(ref new StringStringPair(ref new String(L"Long"), ref new String(c_LongText)));
    m_quickContentItems->Append(ref new StringStringPair(L"Numbering, Bopomofo", L"⒈①Ⅻ〇ㄨㄩ"));
    m_quickContentItems->Append(ref new StringStringPair(L"Unified Han", L"啊阿鼾齄\n丂丄狚狛\n狜狝﨨﨩"));
    m_quickContentItems->Append(ref new StringStringPair(L"Modifiers, punctuation, private use area(PUA)", L"ˊ ˋ ˙ – \n"));
    m_quickContentItems->Append(ref new StringStringPair(L"Unicode Extension A Han", L"㐀㐁䶴䶵"));
    m_quickContentItems->Append(ref new StringStringPair(L"Unicode Extension B Han", L"𠀀𠀁𪛕𪛖"));
    m_quickContentItems->Append(ref new StringStringPair(L"Tibetan", L"དབྱངས་ཅན་སྒྲོལ་དཀར། བཀྲ་ཤིས་རྒྱལ།"));
    m_quickContentItems->Append(ref new StringStringPair(L"Traditional Mongolian", L"ᠣᠺᠵᠡᠵᠺᠰ ᠬᠵᠠᠬᠳᠰᠣᠬᠡᠵᠬ ᠬᠵᠺᠠᠬᠳᠰ ᠺᠬᠣ ᠠ"));
    m_quickContentItems->Append(ref new StringStringPair(L"Yi", L"ꆈꌠꁱꂷꀀꀁꀂꉬꄒꐵꄓꐨ"));
    m_quickContentItems->Append(ref new StringStringPair(L"Dehong Dai / Tai Le", L"ᥘᥣᥭᥰ ᥖᥭᥰ ᥖᥬᥲᥑᥨᥒᥰ"));
    m_quickContentItems->Append(ref new StringStringPair(L"Xishuangbanna Dai / New Tai Lue", L"ᦎᦷᦑᦺᦖᦺᧈᦉᦲᧇᦉᦸᧂᦗᧃᦓᦱ"));
    m_content = m_quickContentItems->GetAt(m_selectedQuickContentIndex)->Value;

    m_quickContentNames = ref new Vector<String^>();
    for (auto quickContentItem : m_quickContentItems)
    {
        m_quickContentNames->Append(quickContentItem->Key);
    }

    m_defaultButtonItems = ref new Vector<String^>();
    m_defaultButtonItems->Append(ref new String(L"None"));
    m_defaultButtonItems->Append(ref new String(L"0"));
    m_defaultButtonItems->Append(ref new String(L"1"));
    m_defaultButtonItems->Append(ref new String(L"2"));
    m_defaultButtonItems->Append(ref new String(L"3"));
    m_defaultButtonItems->Append(ref new String(L"4"));
    m_visibleDefaultButtonItems = ref new Vector<String^>();

    m_cancelButtonItems = ref new Vector<String^>();
    m_cancelButtonItems->Append(ref new String(L"None"));
    m_cancelButtonItems->Append(ref new String(L"0"));
    m_cancelButtonItems->Append(ref new String(L"1"));
    m_cancelButtonItems->Append(ref new String(L"2"));
    m_cancelButtonItems->Append(ref new String(L"3"));
    m_cancelButtonItems->Append(ref new String(L"4"));
    m_visibleCancelButtonItems = ref new Vector<String^>();

    m_allButtonLabels = ref new Vector<StringStringPair^>();
    m_allButtonLabels->Append(ref new StringStringPair(ref new String(L"Button 1 Label"), ref new String(L"Save")));
    m_allButtonLabels->Append(ref new StringStringPair(ref new String(L"Button 2 Label"), ref new String(L"Don't Save")));
    m_allButtonLabels->Append(ref new StringStringPair(ref new String(L"Button 3 Label"), ref new String(L"Cancel")));
    m_visibleButtonLabels = ref new Vector<StringStringPair^>();

    this->SelectedButtonCountIndex = 3;

    this->InitializeComponent();
    this->Loaded += ref new RoutedEventHandler(this, &MessageDialogTester::MainPage::MainPage_Loaded);
}

void MainPage::Title::set(String^ value)
{
    if (!m_title->Equals(value))
    {
        m_title = value;
        this->PropertyChanged(this, ref new PropertyChangedEventArgs(ref new String(L"Title")));
    }
}

void MainPage::MessageContent::set(String^ value)
{
    if (!m_content->Equals(value))
    {
        m_content = value;

        if (!m_isSettingContentFromComboBox)
        {
            m_quickContentItems->GetAt(0)->Value = value;
            this->SelectedQuickContentIndex = 0;
        }

        this->PropertyChanged(this, ref new PropertyChangedEventArgs(ref new String(L"MessageContent")));
    }
}

void MainPage::SelectedQuickContentIndex::set(int32 value)
{
    if (m_selectedQuickContentIndex != value)
    {
        m_selectedQuickContentIndex = value;

        m_isSettingContentFromComboBox = true;
        this->MessageContent = m_quickContentItems->GetAt(static_cast<uint32>(value))->Value;
        m_isSettingContentFromComboBox = false;

        this->PropertyChanged(this, ref new PropertyChangedEventArgs(ref new String(L"SelectedQuickContentIndex")));
    }
}

void MainPage::SelectedButtonCountIndex::set(int32 value)
{
    if (m_selectedButtonCountIndex != value)
    {
        m_selectedButtonCountIndex = value;
        this->PropertyChanged(this, ref new PropertyChangedEventArgs(ref new String(L"SelectedButtonCountIndex")));

        m_visibleButtonLabels->Clear();
        for (uint32 index = 0; index < static_cast<uint32>(m_selectedButtonCountIndex) && index < m_allButtonLabels->Size; ++index)
        {
            m_visibleButtonLabels->Append(m_allButtonLabels->GetAt(index));
        }

        this->SelectedDefaultButtonIndex = this->RefreshButtonIndexItems(m_visibleDefaultButtonItems, m_defaultButtonItems, m_selectedDefaultButtonIndex);
        this->SelectedCancelButtonIndex = this->RefreshButtonIndexItems(m_visibleCancelButtonItems, m_cancelButtonItems, m_selectedCancelButtonIndex);
    }
}

void MainPage::SelectedDefaultButtonIndex::set(int32 value)
{
    m_selectedDefaultButtonIndex = value;
    this->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
    {
        PropertyChanged(this, ref new PropertyChangedEventArgs(ref new String(L"SelectedDefaultButtonIndex")));
    }));
}

void MainPage::SelectedCancelButtonIndex::set(int32 value)
{
    m_selectedCancelButtonIndex = value;
    this->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
    {
        PropertyChanged(this, ref new PropertyChangedEventArgs(ref new String(L"SelectedCancelButtonIndex")));
    }));
}

int32 MainPage::RefreshButtonIndexItems(Vector<String^>^ visibleItems, Vector<String^>^ items, int32 selectedButtonIndex)
{
    visibleItems->Clear();

    for (uint32 index = 0; index <= static_cast<uint32>(m_selectedButtonCountIndex); ++index)
    {
        visibleItems->Append(items->GetAt(index));
    }

    return std::min(selectedButtonIndex, m_selectedButtonCountIndex);
}

void MainPage::MainPage_Loaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e)
{
    this->SelectedButtonCountIndex = 3;
    this->SelectedDefaultButtonIndex = 1;
    this->SelectedCancelButtonIndex = 3;
}

void MainPage::Button_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto dialog = ref new MessageDialog(this->MessageContent, this->Title);
    dialog->DefaultCommandIndex = m_selectedDefaultButtonIndex - 1;
    dialog->CancelCommandIndex = m_selectedCancelButtonIndex - 1;

    dialog->Commands->Clear();
    for (auto label : m_visibleButtonLabels)
    {
        dialog->Commands->Append(ref new UICommand(label->Value));
    }

    #pragma warning (push)
    #pragma warning (disable : 4451)
    create_task(dialog->ShowAsync())
        .then([this, dialog](IUICommand^ chosenCommand)
        {
            uint32 index = 0;
            dialog->Commands->IndexOf(chosenCommand, &index);

            auto resultView = ref new MessageDialogResult();
            resultView->DataContext = ref new MessageDialogResultViewModel(chosenCommand, index);
            resultView->ShowAsync();
        })
        .then([](task<void> task)
        {
            try
            {
                task.wait();
            }
            catch (Platform::Exception^ ex)
            {
                wchar_t buffer[MAX_PATH];
                swprintf_s(buffer, L"dialog->ShowAsync() returned error 0x%08X: %ls\n", ex->HResult, ex->Message->Data());
                OutputDebugStringW(buffer);

                auto exceptionView = ref new MessageDialogException();
                exceptionView->DataContext = ref new MessageDialogExceptionViewModel(ex);
                exceptionView->ShowAsync();
            }
        });
    #pragma warning (pop)
}
