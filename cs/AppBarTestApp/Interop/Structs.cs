using System;
using System.Runtime.InteropServices;

namespace AppBarTestApp.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public APPBARDATA(HandleRef window)
        {
            cbSize = StructureSize;
            WindowHandle = window.Handle;
            CallbackMessage = AppBarCallbackMessage;
            Edge = AppBarEdge.Left;
            Rectangle = new RECT();
            lParam = 0;
        }

        public int cbSize;
        public IntPtr WindowHandle;
        public uint CallbackMessage;
        public AppBarEdge Edge;
        public RECT Rectangle;
        public uint lParam;

        public override string ToString()
        {
            var message = $"lParam={this.lParam}";
            if (this.Rectangle.Width != 0 || this.Rectangle.Height != 0)
            {
                message += $" {this.Rectangle}";
            }

            return message;
        }

        public static uint AppBarCallbackMessage = User32Api.RegisterWindowMessage(nameof(AppBarCallbackMessage));
        public static int StructureSize = Marshal.SizeOf(typeof(APPBARDATA));
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int X
        {
            get { return Left; }
            set { Right -= (Left - value); Left = value; }
        }

        public int Y
        {
            get { return Top; }
            set { Bottom -= (Top - value); Top = value; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public int Width
        {
            get { return Right - Left; }
            set { Right = value + Left; }
        }

        public System.Drawing.Point Location
        {
            get { return new System.Drawing.Point(Left, Top); }
            set { X = value.X; Y = value.Y; }
        }

        public System.Drawing.Size Size
        {
            get { return new System.Drawing.Size(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        public static bool operator ==(RECT r1, RECT r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RECT r1, RECT r2)
        {
            return !r1.Equals(r2);
        }

        public bool Equals(RECT r)
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals(object? obj)
        {
            return obj is RECT ? Equals((RECT)obj) : false;
        }

        public override int GetHashCode()
        {
            return this.X ^ (this.Y << 13 | (int)((uint)this.Y >> 19)) ^ (this.Width << 26 | (int)((uint)this.Width >> 6)) ^ (this.Height << 7 | (int)((uint)this.Height >> 25));
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
        }
    }
}
