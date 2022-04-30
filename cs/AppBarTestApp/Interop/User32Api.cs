using System;
using System.Runtime.InteropServices;

namespace AppBarTestApp.Interop
{
    internal static class User32Api
    {
        const string ApiLib = "user32.dll";

        [DllImport(ApiLib)]
        public static extern uint RegisterWindowMessage(string messageName);

        [DllImport(ApiLib)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow(
            IntPtr hWnd, int x, int y, int width, int height, [MarshalAs(UnmanagedType.Bool)] bool repaint);

        public enum Wm
        {
            Activate = 0x0006,
            WindowPosChanged = 0x0047,
        }
    }
}
