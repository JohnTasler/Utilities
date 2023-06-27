//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

using namespace Platform;
using namespace Platform::Collections;
using namespace Platform::Collections::Details;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Data;

#include "pch.h"
#include "MainPage.g.h"

namespace MessageDialogTester
{
    public ref class StringStringPair sealed
    {
    public:
        StringStringPair() { }
        StringStringPair(String^ key, String^ value)
        {
            this->Key = key;
            this->Value = value;
        }

    public:
        property String^ Key;
        property String^ Value;
    };

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public ref class MainPage sealed : INotifyPropertyChanged
    {
    public:
        MainPage();

    public:
        property MainPage^ ViewModel
        {
            MainPage^ get() { return this; }
        }

        property String^ Title
        {
            String^ get() { return m_title; }
            void set(String^ value);
        }

        property String^ MessageContent
        {
            String^ get() { return m_content; }
            void set(String^ value);
        }

        property IVector<StringStringPair^>^ QuickContentItems
        {
            IVector<StringStringPair^>^ get() { return m_quickContentItems; }
        }

        property int32 SelectedQuickContentIndex
        {
            int32 get() { return m_selectedQuickContentIndex; }
            void set(int32 value);
        }

        property StringStringPair^ SelectedQuickContentValue
        {
            StringStringPair^ get() { return m_quickContentItems->GetAt(m_selectedQuickContentIndex); }
        }

        property int32 SelectedButtonCountIndex
        {
            int32 get() { return m_selectedButtonCountIndex; }
            void set(int32 value);
        }

        property IObservableVector<StringStringPair^>^ ButtonLabels
        {
            IObservableVector<StringStringPair^>^ get() { return m_visibleButtonLabels; }
        }

        property IObservableVector<String^>^ DefaultButtonItems
        {
            IObservableVector<String^>^ get() { return m_visibleDefaultButtonItems; }
        }

        property int32 SelectedDefaultButtonIndex
        {
            int32 get() { return m_selectedDefaultButtonIndex; }
            void set(int32 value);
        }

        property IObservableVector<String^>^ CancelButtonItems
        {
            IObservableVector<String^>^ get() { return m_visibleCancelButtonItems; }
        }

        property int32 SelectedCancelButtonIndex
        {
            int32 get() { return m_selectedCancelButtonIndex; }
            void set(int32 value);
        }

        virtual event PropertyChangedEventHandler^ PropertyChanged;

    private:
        int32 RefreshButtonIndexItems(Vector<String^>^ visibleItems, Vector<String^>^ items, int32 selectedButtonIndex);
        void MainPage_Loaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
        void Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

    private:
        String^ m_title;
        String^ m_content;
        bool m_isSettingContentFromComboBox = false;
        Vector<StringStringPair^>^ m_quickContentItems;
        int32 m_selectedQuickContentIndex = 1;
        Vector<String^>^ m_quickContentNames;
        int32 m_selectedButtonCountIndex = -1;
        Vector<String^>^ m_defaultButtonItems;
        int32 m_selectedDefaultButtonIndex = -1;
        Vector<String^>^ m_cancelButtonItems;
        int32 m_selectedCancelButtonIndex = -1;
        Vector<StringStringPair^>^ m_visibleButtonLabels;
        Vector<StringStringPair^>^ m_allButtonLabels;
        Vector<String^>^ m_visibleDefaultButtonItems;
        Vector<String^>^ m_visibleCancelButtonItems;
    };
}
