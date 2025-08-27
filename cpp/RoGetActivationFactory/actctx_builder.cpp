#include "pch.h"
#include "actctx_builder.h"
#include <taz\environment_utility.h>
#include <taz\resource_loader.h>
#include <taz\string_utility.h>


namespace fs = std::filesystem;

using namespace jtasler;

actctx_builder::actctx_builder(std::wstring_view fileName, std::wstring_view className, std::wstring_view threadingModel)
{
	constexpr auto c_resourceName = L"template.manifest.xml"sv;

	// Load the manifest file template from the module resources
	std::string manifestTemplate
	{
		taz::resource_loader::find_and_load<std::string_view>(nullptr, c_resourceName.data(),
		taz::resource_loader::c_AnsiText.data())
	};

	m_text = taz::string_utility::replace_all(manifestTemplate,
	{
		{ "{{dllFile}}"sv, taz::string_utility::narrow(fileName) },
		{ "{{className}}"sv, taz::string_utility::narrow(className) },
		{ "{{threadingModel}}"sv, taz::string_utility::narrow(threadingModel) },
	});
}

HANDLE actctx_builder::SaveToFile(fs::path manifestFile) const
{
	wil::unique_hfile file;
	file.reset(::CreateFileW(
		manifestFile.c_str(),
		GENERIC_WRITE,
		FILE_SHARE_WRITE,
		nullptr,
		CREATE_ALWAYS,
		FILE_FLAG_SEQUENTIAL_SCAN,
		nullptr));
	THROW_LAST_ERROR_IF(!file);

	DWORD bytesWritten{};
	THROW_IF_WIN32_BOOL_FALSE(::WriteFile(file.get(), m_text.data(), static_cast<uint32_t>(m_text.length()), &bytesWritten, nullptr));
	THROW_IF_WIN32_BOOL_FALSE(::FlushFileBuffers(file.get()));

	return file.release();
}
