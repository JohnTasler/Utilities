#pragma once

#include "StringStringPair.g.h"
#include <mvvm/view_model.h>
#include <mvvm/property_macros.h>

namespace winrt::MessageDialogTester::implementation
{
    struct StringStringPair
			: StringStringPairT<StringStringPair>
			, mvvm::view_model<StringStringPair>
    {
        StringStringPair() = default;
				StringStringPair(hstring key, hstring value)
					: m_propertyKey(key), m_propertyValue(value)
				{
				}

				DEFINE_PROPERTY_NO_NOTIFY(hstring, Key, {})
				DEFINE_PROPERTY_NO_NOTIFY(hstring, Value, {})
    };
}

namespace winrt::MessageDialogTester::factory_implementation
{
    struct StringStringPair : StringStringPairT<StringStringPair, implementation::StringStringPair>
    {
    };
}
