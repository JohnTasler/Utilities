//
// MessageDialogException.xaml.h
// Declaration of the MessageDialogException class
//

#pragma once

#include "pch.h"
#include "MessageDialogException.g.h"

using namespace Windows::UI::Popups;

namespace MessageDialogTester
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MessageDialogExceptionViewModel sealed
    {
    public:
        MessageDialogExceptionViewModel(Platform::Exception^ exception)
        {
            this->Exception = exception;

            wchar_t buffer[32];
            swprintf_s(buffer, L"0x%08X", exception->HResult);
            this->HResultFormatted = ref new Platform::String(buffer);
        }

        property Platform::Exception^ Exception;
        property Platform::String^ HResultFormatted;
    };


    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MessageDialogException sealed
    {
    public:
        MessageDialogException();

        property MessageDialogExceptionViewModel^ ViewModel
        {
            MessageDialogExceptionViewModel^ get()
            {
                return dynamic_cast<MessageDialogExceptionViewModel^>(this->DataContext);
            }
        }
    };
}
