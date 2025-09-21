#include "pch.h"
#include "Program.h"
#include "taz/resource_loader.h"

namespace winrt
{
	using namespace winrt::Windows::Foundation;
}
using namespace winrt;
using namespace jtasler;
using namespace jtasler::util::RoGetActivationFactory;

Program::Program(int32_t argc, wchar_t* argv[])
{
	m_args.reserve(argc);
	for (auto arg : wil::make_range(argv, argv + argc))
	{
		m_args.emplace_back(arg);
	}
}

int32_t Program::Run()
{
	ParseCommandLine();
	if (auto validationResult = ValidateCommandLine(); validationResult != 0)
	{
		return validationResult;
	}

	try
	{
		// Initialize WinRT after we've possibly created an activation context
		winrt::init_apartment();

		winrt::IInspectable factory{ nullptr };
		if (m_dllName.size())
		{
			factory = LoadActivationFactory();
		}
		else
		{
			factory = get_activation_factory<winrt::IInspectable>(m_className);
		}
		wprintf(L"Class Name    : %ls\n", FormatClassName(factory).data());

		auto trustLevel = get_trust_level(factory);
		wprintf(L"  Trust Level : %i %ls\n", static_cast<int32_t>(trustLevel), FormatTrustLevelText(trustLevel).data());

		auto iids = get_interfaces(factory);
		wprintf(L"  Interfaces  : %u\n", iids.size());
		for (auto&& iid : iids)
		{
			wprintf(L"    %ls\n", FormatInterfaceText(iid).c_str());
		}
	}
	catch (const wil::ResultException& ex)
	{
		auto code = ex.GetErrorCode();
		auto message = ex.what();

		wprintf(
			L"ERROR     : %ls\n"
			L"  hresult : 0x%08X (%i)\n"
			L"  message : %hs\n\n",
			m_className.data(),
			static_cast<uint32_t>(code),
			static_cast<uint32_t>(code),
			message);

		return code;
	}
	catch (const hresult_error& ex)
	{
		auto code = ex.code();
		auto message = ex.message();

		wprintf(
			L"ERROR     : %ls\n"
			L"  hresult : 0x%08X (%i)\n"
			L"  message : %ls\n\n",
			m_className.data(),
			static_cast<uint32_t>(code),
			static_cast<uint32_t>(code),
			message.c_str());

		return ex.code();
	}

	return 0;
}

winrt::IInspectable Program::LoadActivationFactory()
{
	wil::unique_hmodule module;
	module.reset(::LoadLibraryW(m_dllName.data()));
	THROW_LAST_ERROR_IF_NULL(module);

	using factory_prototype = PFNGETACTIVATIONFACTORY;

	auto dllGetActivationFactory =
		reinterpret_cast<factory_prototype>(GetProcAddress(module.get(), "DllGetActivationFactory"));
	THROW_HR_IF_NULL(E_NOTIMPL, dllGetActivationFactory);

	HSTRING_HEADER header{};
	HSTRING className{};
	THROW_IF_FAILED(::WindowsCreateStringReference(m_className.data(), static_cast<uint32_t>(m_className.size()), &header, &className));

	wil::com_ptr_t<::IActivationFactory> factory;
	THROW_IF_FAILED(dllGetActivationFactory(className, factory.put()));

	return { factory.detach(), {} };
}

void Program::ParseCommandLine()
{
	const auto argc = m_args.size();

	if (argc > 1)
	{
		m_className = m_args[1];

		if (argc > 2)
		{
			m_dllName = m_args[2];

			if (argc > 3)
			{
				const auto& threadingModelSwitch = m_args[3];
				if (threadingModelSwitch == L"-b" || threadingModelSwitch == L"--both" || threadingModelSwitch == L"/b" || threadingModelSwitch == L"/both")
				{
					m_threadingModel = L"both"sv;
				}
				else if (threadingModelSwitch == L"-s" || threadingModelSwitch == L"--sta" || threadingModelSwitch == L"/s" || threadingModelSwitch == L"/sta")
				{
					m_threadingModel = L"sta"sv;
				}
				else if (threadingModelSwitch == L"-m" || threadingModelSwitch == L"--mta" || threadingModelSwitch == L"/m" || threadingModelSwitch == L"/mta")
				{
					m_threadingModel = L"mta"sv;
				}
			}
			else
			{
				m_threadingModel = L"both"sv;
			}
		}
	}
}

int32_t Program::ValidateCommandLine()
{
	const auto argc = m_args.size();

	if (argc > 4)
	{
		return ShowSyntax(L"ERROR: Too many arguments specified", -3);
	}

	if (m_className.size() == 0)
	{
		return ShowSyntax(L"ERROR: No activatable_class_name specified", -4);
	}

	if (m_dllName.size() > 0)
	{
		if (m_threadingModel.size() == 0)
		{
			return ShowSyntax(L"ERROR: Invalid threading model specified", -5);
		}
	}

	return 0;
}

int32_t Program::ShowSyntax(PCWSTR message, int32_t result)
{
	auto syntax = taz::resource_loader::find_and_load<std::string_view>(nullptr, L"syntax.txt", L"ANSI_TEXT");
	wprintf(L"%ls\n\n%hs\n", message ? message : L"", syntax.data());
	return result;
}

std::wstring Program::FormatClassName(winrt::IInspectable const& factory)
{
	try
	{
		return get_class_name(factory).c_str();
	}
	catch (...)
	{
		return m_className.data();
	}
}

std::wstring_view Program::FormatTrustLevelText(winrt::TrustLevel trustLevel)
{
	switch (trustLevel)
	{
	case winrt::TrustLevel::BaseTrust: return L"BaseTrust"sv;
	case winrt::TrustLevel::PartialTrust: return L"PartialTrust"sv;
	case winrt::TrustLevel::FullTrust: return L"FullTrust"sv;
	default: return L"--Trust Level unrecognized--"sv;
	}
}

std::wstring Program::FormatInterfaceText(guid const& iid)
{
	static constexpr std::pair<guid, std::wstring_view> c_commonIIDs[] =
	{
		{ guid{"00000035-0000-0000-C000-000000000046"sv}, L"IActivationFactory"sv},
		{ guid{"ECABE149-4833-452B-8112-3EEEC6D20CB0"sv}, L"IValueUnmarshalByPropertySet"sv},
	};

	auto iidString = to_hstring(iid);
	std::wstring result{ iidString };

	for (auto&& mapping : c_commonIIDs)
	{
		if (iid == mapping.first)
		{
			result.append(L" ").append(mapping.second);
			return result;
		}
	}

	// Look in the registry
	if (!m_interfaceKey)
	{
		::RegOpenKeyExW(HKEY_CLASSES_ROOT, L"Interface", 0, STANDARD_RIGHTS_READ | KEY_QUERY_VALUE, &m_interfaceKey);
	}

	DWORD byteCount = 0;
	if (ERROR_SUCCESS == ::RegGetValueW(m_interfaceKey.get(), iidString.c_str(), NULL, RRF_RT_REG_SZ, NULL, NULL, &byteCount))
	{
		std::wstring interfaceName(byteCount / sizeof(wchar_t), L'\0');
		if (ERROR_SUCCESS == ::RegGetValueW(m_interfaceKey.get(), iidString.c_str(), NULL, RRF_RT_REG_SZ, NULL, (PBYTE)interfaceName.data(), &byteCount))
		{
			interfaceName.resize(wcslen(interfaceName.data()));
			result.append(L" ").append(interfaceName);
		}
	}

	return result;
}
