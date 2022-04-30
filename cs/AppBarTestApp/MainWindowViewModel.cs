using AppBarTestApp.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace AppBarTestApp
{
    internal class Notification
    {
        public Notification(string? message)
        {
            this.Message = message;
        }

        public Notification(AppBarNotification appBarNotification, string? message)
            : this(message)
        {
            this.AppBarNotification = appBarNotification;
        }

        public AppBarNotification AppBarNotification { get; private set; }

        public string? Message { get; private set; }
    }

    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public RECT Position
        {
            get => m_position;
            private set
            {
                if (m_position != value)
                {
                    m_position = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionText)));
                }
            }
        }
        private RECT m_position;

        public AppBarEdge[] Edges => new[] { AppBarEdge.Left, AppBarEdge.Right, AppBarEdge.Top, AppBarEdge.Bottom };

        public AppBarEdge Edge
        {
            get { return m_edge; }
            set
            {
                if (m_edge != value)
                {
                    m_edge = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Edge)));
                    this.RepositionWindow();
                }
            }
        }
        private AppBarEdge m_edge = AppBarEdge.Right;

        public string PositionText => $"{Position.X},{Position.Y} Size={Position.Width},{Position.Height}";

        public ObservableCollection<Notification> Notifications { get; } = new ObservableCollection<Notification>();

        public bool IsPinned
        {
            get { return m_isPinned; }
            set
            {
                if (m_isPinned != value)
                {
                    m_isPinned = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPinned)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PinCharacter)));
                    RepositionWindow();
                }
            }
        }
        private bool m_isPinned = true;

        public string PinCharacter => this.IsPinned ? "\xE196" : "\xE141";

        internal void RegisterAppBar(HwndSource source)
        {
            m_hwndRef = source.CreateHandleRef();
            source.AddHook(this.HwndSourceHook);

            var data = new APPBARDATA(m_hwndRef);
            SendAppBarMessage(AppBarMessage.New, ref data);

            this.RepositionWindow();
        }

        internal void UnregisterAppBar()
        {
            var data = new APPBARDATA(m_hwndRef);
            SendAppBarMessage(AppBarMessage.Remove, ref data);
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (handled)
            {
                return IntPtr.Zero;
            }

            if (msg != APPBARDATA.AppBarCallbackMessage)
            {
                var data = new APPBARDATA();

                switch ((User32Api.Wm)msg)
                {
                    case User32Api.Wm.Activate:
                        data.lParam = (uint)wParam.ToInt32();
                        SendAppBarMessage(AppBarMessage.Activate, ref data);
                        return (IntPtr)1;

                    case User32Api.Wm.WindowPosChanged:
                        SendAppBarMessage(AppBarMessage.WindowPosChanged, ref data);
                        return (IntPtr)1;

                    default:
                        return IntPtr.Zero;
                }
            }

            var notification = (Interop.AppBarNotification)wParam.ToInt32();
            var messageDetail = Enum.GetName(notification);
            switch (notification)
            {
                case Interop.AppBarNotification.FullScreenApp:
                    messageDetail += $" opening={lParam != IntPtr.Zero}";
                    break;
                case AppBarNotification.StateChange:
                    break;
                case AppBarNotification.PosChanged:
                    this.RepositionWindow();
                    break;
                case AppBarNotification.WindowArrange:
                    messageDetail += $" beginningArrange={lParam != IntPtr.Zero}";
                    break;
                default:
                    throw new ArgumentException($"Invalid notification message value: wParam={wParam.ToInt32()}", nameof(wParam));
            }

            this.Notifications.Add(new Notification(notification, messageDetail));

            handled = true;
            return IntPtr.Zero;
        }

        private void RepositionWindow()
        {
            var data = new APPBARDATA(m_hwndRef);
            data.Edge = m_edge;
            data.Rectangle = ComputePosition();
            data.Edge = m_edge;

            SendAppBarMessage(AppBarMessage.QueryPos, ref data);
            SendAppBarMessage(AppBarMessage.SetPos, ref data);

            User32Api.MoveWindow(m_hwndRef.Handle, data.Rectangle.Left, data.Rectangle.Top, data.Rectangle.Width, data.Rectangle.Height, true);
            this.Position = data.Rectangle;
        }

        private void SendAppBarMessage(AppBarMessage message, ref APPBARDATA data)
        {
            string messageDetail = $"Sending {Enum.GetName(message)} {data}";
            Notifications.Add(new Notification(messageDetail));
            Shell32Api.SHAppBarMessage(message, ref data);
        }

        private RECT ComputePosition()
        {
            var dpi = VisualTreeHelper.GetDpi(Application.Current.MainWindow);
            var screenWidth = (int)(SystemParameters.PrimaryScreenWidth * dpi.DpiScaleX);
            var screenHeight = (int)(SystemParameters.PrimaryScreenHeight * dpi.DpiScaleY);

            var width = (int)(m_width * dpi.DpiScaleX);
            var height = (int)(m_height * dpi.DpiScaleY);

            var data = new APPBARDATA(m_hwndRef);
            SendAppBarMessage(AppBarMessage.GetTaskbarPos, ref data);

            switch (m_edge)
            {
                case AppBarEdge.Left:
                    return new RECT(0, 0, width, screenHeight);
                case AppBarEdge.Right:
                    return new RECT(screenWidth - width, 0, screenWidth, screenHeight);
                case AppBarEdge.Top:
                    return new RECT(0, 0, screenWidth, height);
                case AppBarEdge.Bottom:
                    return new RECT(0, screenHeight - height - data.Rectangle.Height, screenWidth, screenHeight - data.Rectangle.Height);
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_edge));
            }
        }

        private HandleRef m_hwndRef;
        private int m_width = 172;
        private int m_height = 128;
    }
}
