#pragma once

namespace jtasler::util::RoGetActivationFactory
{
	struct Program
	{
		Program(int32_t argc, wchar_t* argv[]);
		int32_t Run();

	private:
		winrt::Windows::Foundation::IInspectable LoadActivationFactory();
		void ParseCommandLine();
		int32_t ValidateCommandLine();
		int32_t ShowSyntax(PCWSTR message = nullptr, int32_t result = -2);

		std::wstring FormatClassName(winrt::Windows::Foundation::IInspectable const& factory);
		std::wstring_view FormatTrustLevelText(winrt::Windows::Foundation::TrustLevel trustLevel);
		std::wstring FormatInterfaceText(winrt::guid const& iid);

	private:
		//actctx context;
		std::vector<std::wstring_view> m_args;
		std::wstring_view m_className;
		std::wstring_view m_dllName;
		std::wstring_view m_threadingModel;
		wil::unique_hkey m_interfaceKey;
	};
}
