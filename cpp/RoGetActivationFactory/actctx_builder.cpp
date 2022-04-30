#include "pch.h"
#include "actctx_builder.h"
#include <environment_utility.h>
#include <resource_loader.h>
#include <string_utility.h>

using namespace jtasler;

actctx_builder::actctx_builder(std::wstring_view fileName, std::wstring_view className, std::wstring_view threadingModel)
{
    constexpr auto c_resourceName = L"template.manifest.xml"sv;

    // Load the manifest file template from the modeul resources
    std::string manifestTemplate{ resource_loader::find_and_load<std::string_view>(nullptr, c_resourceName.data(), resource_loader::c_AnsiText.data()) };

    m_text = string_utility::replace_all(manifestTemplate,
    {
        { "{{dllFile}}"sv, string_utility::narrow(fileName) },
        { "{{className}}"sv, string_utility::narrow(className) },
        { "{{threadingModel}}"sv, string_utility::narrow(threadingModel) },
    });
}

std::wstring actctx_builder::SaveToFile() const
{
    auto tempFileName = environment_utility::get_variable(L"TEMP")
        .append(string_utility::widen(std::filesystem::path::preferred_separator))
        .append(string_utility::replace_all(std::wstring_view(winrt::to_hstring(winrt::Windows::Foundation::GuidHelper::CreateNewGuid())),
            {
                { L"-"sv, L""sv },
                { L"{"sv, L""sv },
                { L"}"sv, L""sv },
            }))
        .append(L".manifest.xml");

    wil::unique_hfile file;
    file.reset(::CreateFileW(
        tempFileName.c_str(),
        GENERIC_WRITE,
        FILE_SHARE_WRITE,
        nullptr,
        CREATE_ALWAYS,
        FILE_ATTRIBUTE_TEMPORARY | FILE_FLAG_SEQUENTIAL_SCAN,
        nullptr));
    THROW_LAST_ERROR_IF(!file);

    DWORD bytesWritten{};
    THROW_IF_WIN32_BOOL_FALSE(::WriteFile(file.get(), m_text.data(), static_cast<uint32_t>(m_text.length()), &bytesWritten, nullptr));
    THROW_IF_WIN32_BOOL_FALSE(::FlushFileBuffers(file.get()));

    return tempFileName;
}
