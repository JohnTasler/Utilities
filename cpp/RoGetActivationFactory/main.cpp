#include "pch.h"
#include "Program.h"

using namespace jtasler::util::RoGetActivationFactory;

int32_t wmain(int32_t argc, wchar_t* argv[]) noexcept try
{
	Program program{ argc, argv };
	return program.Run();
}
catch (const wil::ResultException& ex)
{
	wprintf(L"ERROR: 0x%08X\n%hs\n%ls\n",
		static_cast<uint32_t>(ex.GetErrorCode()),
		ex.what(),
		ex.GetFailureInfo().pszMessage);
	return ex.GetErrorCode();
}
catch (const winrt::hresult_error& ex)
{
	wprintf(L"ERROR: 0x%08X\n%ls\n",
		static_cast<uint32_t>(ex.code()),
		ex.message().c_str());
	return ex.code();
}
catch (const std::exception& ex)
{
	wprintf(L"ERROR: %hs\n", ex.what());
	RETURN_CAUGHT_EXCEPTION();
}
catch (...)
{
	RETURN_CAUGHT_EXCEPTION();
}
