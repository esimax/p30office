using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.ApplicationServices;

namespace POL.Lib.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NotifyIconData
    {
        public uint cbSize;

        public IntPtr WindowHandle;

        public uint TaskbarIconId;

        public EnumIconDataMembers ValidMembers;

        public uint CallbackMessageId;

        public IntPtr IconHandle;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string ToolTipText;


        public EnumIconState IconState;

        public EnumIconState StateMask;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string BalloonText;

        public uint VersionOrTimeout;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string BalloonTitle;

        public EnumBalloonFlags BalloonFlags;

        public Guid TaskbarIconGuid;

        public IntPtr CustomBalloonIconHandle;


        public static NotifyIconData CreateDefault(IntPtr handle)
        {
            var data = new NotifyIconData();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                data.cbSize = (uint) Marshal.SizeOf(data);
            }
            else
            {
                data.cbSize = 504;

                data.VersionOrTimeout = 10;
            }

            data.WindowHandle = handle;
            data.TaskbarIconId = 0x0;
            data.CallbackMessageId = WindowMessageSink.CallbackMessageId;
            data.VersionOrTimeout = (uint) EnumNotifyIconVersion.Win95;

            data.IconHandle = IntPtr.Zero;

            data.IconState = EnumIconState.Hidden;
            data.StateMask = EnumIconState.Hidden;

            data.ValidMembers = EnumIconDataMembers.Message
                                | EnumIconDataMembers.Icon
                                | EnumIconDataMembers.Tip;

            data.ToolTipText = data.BalloonText = data.BalloonTitle = string.Empty;

            return data;
        }
    }
}
