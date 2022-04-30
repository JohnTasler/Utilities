#pragma once

namespace jtasler
{
    struct actctx_builder
    {
        actctx_builder() = delete;
        actctx_builder(actctx_builder&&) = delete;
        actctx_builder(std::wstring_view fileName, std::wstring_view className, std::wstring_view threadingModel);

        std::wstring SaveToFile() const;

    private:
        std::string m_text;
    };
}
