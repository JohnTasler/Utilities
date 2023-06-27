using Windows.System;

namespace AppDiagnosticInfoTestApp
{
    public class AppResourceGroupMemoryReportViewModel
    {
        public AppResourceGroupMemoryReportViewModel(AppResourceGroupMemoryReport model)
        {
            this.Model = model;
        }

        public AppResourceGroupMemoryReport Model { get; }

        public string CommitUsageLevel => this.Model != null ? this.Model.CommitUsageLevel.ToString() : string.Empty;

        public string CommitUsageLimit => this.Model != null ? FormatBytes(this.Model.CommitUsageLimit) : string.Empty;

        public string PrivateCommitUsage => this.Model != null ? FormatBytes(this.Model.PrivateCommitUsage) : string.Empty;

        public string TotalCommitUsage => this.Model != null ? FormatBytes(this.Model.TotalCommitUsage) : string.Empty;

        private static string FormatBytes(ulong byteCount)
        {
            if (byteCount == ulong.MaxValue)
            {
                return string.Empty;
            }

            if (byteCount > 0x3FFFFFFF)
            {
                return ((float)byteCount / 1024 / 1024 / 1024).ToString("N3") + " GB";
            }

            if (byteCount > 0x000FFFFF)
            {
                return ((float)byteCount / 1024 / 1024).ToString("N3") + " MB";
            }

            if (byteCount > 0x000003FF)
            {
                return ((float)byteCount / 1024).ToString("N3") + " KB";
            }

            return byteCount.ToString("N0") + " Bytes";
        }
    }
}
