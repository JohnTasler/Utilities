//
// MessageDialogResult.xaml.h
// Declaration of the MessageDialogResult class
//

#pragma once

#include "pch.h"
#include "MessageDialogResult.g.h"

using namespace Windows::UI::Popups;

namespace MessageDialogTester
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MessageDialogResultViewModel sealed
    {
    public:
        MessageDialogResultViewModel(IUICommand^ command, uint32 index)
        {
            this->Command = command;
            this->Index = index;
        }

        property IUICommand^ Command;

        property uint32 Index;
    };


    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MessageDialogResult sealed
    {
    public:
        MessageDialogResult();

        property MessageDialogResultViewModel^ ViewModel
        {
            MessageDialogResultViewModel^ get()
            {
                return dynamic_cast<MessageDialogResultViewModel^>(this->DataContext);
            }
        }
    };
}
