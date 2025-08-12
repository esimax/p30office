using System;

namespace POL.Lib.Utils
{
    [Flags]
    public enum ExtendedWindowStyles
    {
        AcceptFiles = 16,
        AppWindow = 262144,
        ClientEdge = 512,
        Composite = 33554432,
        ContextHelp = 1024,
        ControlParent = 65536,
        DialogModalFrame = 1,
        Layered = 524288,
        RTLLayout = 4194304,
        LeftAlign = 0,
        LeftScrollBar = 16384,
        LTRReading = 0,
        MDIChild = 64,
        NoActivate = 134217728,
        NotInheritLayout = 1048576,
        NoParentNotify = 4,
        OverlappedWindow = 768,
        PaletteWindow = 392,
        RightAlign = 4096,
        RightScrollBar = 0,
        RTLReading = 8192,
        StaticEdge = 131072,
        ToolWindow = 128,
        Topmost = 8,
        Transparent = 32,
        WindowEdge = 256
    }
}
