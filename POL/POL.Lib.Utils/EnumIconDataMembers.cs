using System;

namespace POL.Lib.Utils
{
    [Flags]
    public enum EnumIconDataMembers
    {
        Message = 0x01,

        Icon = 0x02,

        Tip = 0x04,

        State = 0x08,

        Info = 0x10,

        Realtime = 0x40,

        UseLegacyToolTips = 0x80
    }
}
