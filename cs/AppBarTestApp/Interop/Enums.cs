
namespace AppBarTestApp.Interop
{
    public enum AppBarMessage : uint
    {
        New = 0x00000000,
        Remove = 0x00000001,
        QueryPos = 0x00000002,
        SetPos = 0x00000003,
        GetState = 0x00000004,
        GetTaskbarPos = 0x00000005,
        Activate = 0x00000006,
        GetAutohideBar = 0x00000007,
        SetAutohideBar = 0x00000008,
        WindowPosChanged = 0x0000009,
        SetState = 0x0000000a,
        GetAutoHideBarEx = 0x0000000b,
        SetAutoHideBarEx = 0x0000000c,
    }

    public enum AppBarNotification : uint
    {
        StateChange = 0x0000000,
        PosChanged = 0x0000001,
        FullScreenApp = 0x0000002,
        WindowArrange = 0x0000003,
    }

    public enum AppBarState
    {
        AutoHide = 0x0000001,
        AlwaysOnTop = 0x0000002,
    }

    public enum AppBarEdge : uint
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
    }
}
