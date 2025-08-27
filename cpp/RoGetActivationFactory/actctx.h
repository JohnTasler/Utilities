#include "pch.h"

namespace jtasler
{
	using unique_actctx = wil::unique_any_handle_invalid<decltype(&::ReleaseActCtx), ::ReleaseActCtx>;
	//    using unique_actctx_activation = wil::unique_any_null<ULONG_PTR, decltype(&::DeactivateActCtx), ::DeactivateActCtx>; // Needs more parameters

	struct actctx
	{
		actctx() = default;
		actctx(actctx&& other) noexcept
			: m_actctx(std::move(other.m_actctx))
			, m_activationCookie(std::move(other.m_activationCookie))
		{
			other.m_activationCookie = {};
		}

		actctx& operator=(actctx&& other) noexcept
		{
			std::swap(m_actctx, other.m_actctx);
			std::swap(m_activationCookie, other.m_activationCookie);
			return *this;
		}

		actctx(const std::wstring_view& manifestFileName)
		{
			std::wstring fileNameString;
			std::wstring_view fileName = (manifestFileName.data()[manifestFileName.size()] != L'\0')
				? (fileNameString = manifestFileName)
				: manifestFileName;

			ACTCTXW actctxw{};
			actctxw.cbSize = sizeof(actctxw);
			actctxw.dwFlags = 0; // ACTCTX_FLAG_SET_PROCESS_DEFAULT;
			actctxw.lpSource = fileName.data();

			m_actctx.reset(::CreateActCtxW(&actctxw));
			THROW_LAST_ERROR_IF(!m_actctx);
			THROW_LAST_ERROR_IF(!::ActivateActCtx(m_actctx.get(), &m_activationCookie));
		}

		~actctx()
		{
			if (m_activationCookie)
			{
				THROW_LAST_ERROR_IF(!::DeactivateActCtx(0, m_activationCookie));
				m_activationCookie = {};
			}
		}

	private:
		unique_actctx m_actctx;
		ULONG_PTR m_activationCookie{};
	};
}
