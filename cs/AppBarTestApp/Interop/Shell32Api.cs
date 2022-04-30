using System;
using System.Runtime.InteropServices;

namespace AppBarTestApp.Interop
{
    internal static class Shell32Api
    {
        const string ApiLib = "shell32.dll";

        [DllImport("shell32.dll")]
        public static extern IntPtr SHAppBarMessage(AppBarMessage appBarMessage, [In] ref APPBARDATA pData);
    }
}
