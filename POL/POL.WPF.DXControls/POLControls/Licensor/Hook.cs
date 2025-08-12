using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace POL.WPF.Licensor
{

    #region Interface

    public interface IHookController
    {
        IntPtr OwnerHandle { get; }
        bool InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam);
        bool InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam);
    }

    public interface IHookController2 : IHookController
    {
        void WndGetMessage(ref Message msg);
    }

    #endregion

    #region Delegate

    public delegate void MsgEventHandler(object sender, ref Message msg);

    public delegate int Hook(int ncode, IntPtr wParam, IntPtr lParam);

    #endregion

    #region Class

    public class HookInfo
    {
        public IntPtr getMessageHookHandle;
        public Hook getMessageHookProc;
        public bool inHook;
        public bool inMouseHook;
        private HookManager manager;
        public IntPtr mouseHookHandle;
        public Hook mouseHookProc;
        public IntPtr wndHookHandle;
        public Hook wndHookProc;
        public HookInfo(HookManager manager)
        {
            this.manager = manager;
            inMouseHook = false;
            inHook = false;
            wndHookHandle = getMessageHookHandle = mouseHookHandle = IntPtr.Zero;
            wndHookProc = mouseHookProc = getMessageHookProc = null;
            ThreadId = HookManager.GetCurrentThreadId();
            HookControllers = new ArrayList();
        }

        public int ThreadId { get; }

        public ArrayList HookControllers { get; }
    }

    public class HookManager
    {
        public ArrayList HookControllers;

        static HookManager()
        {
            DefaultManager = new HookManager();
        }

        public HookManager()
        {
            Application.ApplicationExit += OnApplicationExit;
            Application.ThreadExit += OnThreadExit;
            HookHash = new Hashtable();
            HookControllers = new ArrayList();
        }

        public static int CurrentThread
        {
            get { return GetCurrentThreadId(); }
        }

        public static HookManager DefaultManager { get; }

        public Hashtable HookHash { get; }

        public void AddController(IHookController ctrl)
        {
            var info1 = GetInfoByThread();
            info1.HookControllers.Add(ctrl);
            if (info1.HookControllers.Count == 1)
            {
                InstallHook(info1);
            }
        }

        public void CheckController(IHookController ctrl)
        {
            var info1 = GetInfoByThread();
            if (!info1.HookControllers.Contains(ctrl))
            {
                AddController(ctrl);
            }
        }

        ~HookManager()
        {
            RemoveHooks();
            Application.ApplicationExit -= OnApplicationExit;
            Application.ThreadExit -= OnThreadExit;
        }

        protected virtual HookInfo GetInfoByThread()
        {
            var num1 = CurrentThread;
            var info1 = HookHash[num1] as HookInfo;
            if (info1 == null)
            {
                info1 = new HookInfo(this);
                HookHash[num1] = info1;
            }
            return info1;
        }

        protected int GetMessageHook(int ncode, IntPtr wParam, IntPtr lParam)
        {
            var info1 = GetInfoByThread();
            var api_msg1 = (API_MSG) Marshal.PtrToStructure(lParam, typeof (API_MSG));
            if (!info1.inHook && (lParam != IntPtr.Zero))
            {
                try
                {
                    info1.inHook = true;
                    InternalGetMessage(ref api_msg1);
                }
                finally
                {
                    info1.inHook = false;
                }
            }
            return CallNextHookEx(info1.wndHookHandle, ncode, wParam, lParam);
        }

        internal void InstallHook(HookInfo hInfo)
        {
            if (hInfo.wndHookHandle == IntPtr.Zero)
            {
                hInfo.mouseHookProc = MouseHook;
                hInfo.wndHookProc = WndHook;
                hInfo.getMessageHookProc = GetMessageHook;
                hInfo.wndHookHandle = SetWindowsHookEx(4, hInfo.wndHookProc, 0, hInfo.ThreadId);
                hInfo.mouseHookHandle = SetWindowsHookEx(7, hInfo.mouseHookProc, 0, hInfo.ThreadId);
                hInfo.getMessageHookHandle = IntPtr.Zero;
            }
        }

        internal void InternalGetMessage(ref API_MSG msg)
        {
            var info1 = GetInfoByThread();
            for (var num1 = 0; num1 < info1.HookControllers.Count; num1++)
            {
                var controller1 = info1.HookControllers[num1] as IHookController2;
                if (controller1 != null)
                {
                    var message1 = msg.ToMessage();
                    controller1.WndGetMessage(ref message1);
                    msg.FromMessage(ref message1);
                }
            }
        }

        internal bool InternalPostFilterMessage(HookInfo hInfo, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam,
            IntPtr LParam)
        {
            var flag1 = false;
            for (var num1 = hInfo.HookControllers.Count - 1; num1 >= 0; num1--)
            {
                var controller1 = hInfo.HookControllers[num1] as IHookController;
                flag1 |= controller1.InternalPostFilterMessage(Msg, wnd, HWnd, WParam, LParam);
                if ((Msg == 2) && (controller1.OwnerHandle == HWnd))
                {
                    RemoveController(controller1);
                }
            }
            return flag1;
        }

        internal bool InternalPreFilterMessage(HookInfo hInfo, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam,
            IntPtr LParam)
        {
            var flag1 = false;
            for (var num1 = 0; num1 < hInfo.HookControllers.Count; num1++)
            {
                var controller1 = hInfo.HookControllers[num1] as IHookController;
                flag1 |= controller1.InternalPreFilterMessage(Msg, wnd, HWnd, WParam, LParam);
            }
            return flag1;
        }

        protected int MouseHook(int ncode, IntPtr wParam, IntPtr lParam)
        {
            var info1 = GetInfoByThread();
            var num1 = 0;
            var flag1 = true;
            if (ncode == 0)
            {
                var mousehookstruct1 = (MOUSEHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof (MOUSEHOOKSTRUCT));
                if (!info1.inMouseHook && (lParam != IntPtr.Zero))
                {
                    try
                    {
                        var control1 = Control.FromHandle(mousehookstruct1.hwnd);
                        info1.inMouseHook = true;
                        flag1 =
                            !InternalPreFilterMessage(info1, wParam.ToInt32(), control1, mousehookstruct1.hwnd,
                                IntPtr.Zero, new IntPtr((mousehookstruct1.Pt.X << 0x10) | mousehookstruct1.Pt.Y));
                        goto Label_00AB;
                    }
                    finally
                    {
                        info1.inMouseHook = false;
                    }
                }
                return CallNextHookEx(info1.mouseHookHandle, ncode, wParam, lParam);
            }
            Label_00AB:
            num1 = CallNextHookEx(info1.mouseHookHandle, ncode, wParam, lParam);
            if (!flag1)
            {
                num1 = -1;
            }
            return num1;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            Application.ThreadExit -= OnThreadExit;
            Application.ApplicationExit -= OnApplicationExit;
            RemoveHooks();
        }

        private void OnThreadExit(object sender, EventArgs e)
        {
            RemoveHook(GetInfoByThread(), true);
        }

        public void RemoveController(IHookController ctrl)
        {
            var info1 = GetInfoByThread();
            info1.HookControllers.Remove(ctrl);
            if (info1.HookControllers.Count == 0)
            {
                RemoveHook(info1, false);
            }
        }

        internal void RemoveHook(HookInfo hInfo, bool disposing)
        {
            if (hInfo.wndHookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hInfo.wndHookHandle);
                hInfo.wndHookHandle = IntPtr.Zero;
                hInfo.wndHookProc = null;
                hInfo.getMessageHookHandle = IntPtr.Zero;
                hInfo.getMessageHookProc = null;
                UnhookWindowsHookEx(hInfo.mouseHookHandle);
                hInfo.mouseHookHandle = IntPtr.Zero;
                hInfo.mouseHookProc = null;
                HookHash.Remove(hInfo.ThreadId);
            }
        }

        protected virtual void RemoveHooks()
        {
            var list1 = new ArrayList();
            foreach (HookInfo info1 in HookHash.Values)
            {
                list1.Add(info1);
            }
            HookHash.Clear();
            for (var num1 = 0; num1 < list1.Count; num1++)
            {
                RemoveHook(list1[num1] as HookInfo, true);
            }
        }

        protected int WndHook(int ncode, IntPtr wParam, IntPtr lParam)
        {
            var info1 = GetInfoByThread();
            var num1 = 0;
            var cwpstruct1 = (CWPSTRUCT) Marshal.PtrToStructure(lParam, typeof (CWPSTRUCT));
            Control control1 = null;
            try
            {
                if (!info1.inHook && (lParam != IntPtr.Zero))
                {
                    try
                    {
                        control1 = Control.FromHandle(cwpstruct1.hwnd);
                        info1.inHook = true;
                        num1 = InternalPreFilterMessage(info1, cwpstruct1.message, control1, cwpstruct1.hwnd,
                            cwpstruct1.wParam, cwpstruct1.lParam)
                            ? 1
                            : 0;
                        goto Label_0091;
                    }
                    finally
                    {
                        info1.inHook = false;
                    }
                }
                return CallNextHookEx(info1.wndHookHandle, ncode, wParam, lParam);
                Label_0091:
                num1 = CallNextHookEx(info1.wndHookHandle, ncode, wParam, lParam);
            }
            finally
            {
                InternalPostFilterMessage(info1, cwpstruct1.message, control1, cwpstruct1.hwnd, cwpstruct1.wParam,
                    cwpstruct1.lParam);
            }
            return num1;
        }


        public static void RemoveAllHooks()
        {
            DefaultManager.RemoveHooks();
        }


        [DllImport("USER32.dll", CharSet = CharSet.Auto)]
        protected static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetCurrentThreadId();

        [DllImport("USER32.dll", CharSet = CharSet.Auto)]
        protected static extern IntPtr SetWindowsHookEx(int idHook, Hook lpfn, int hMod, int dwThreadId);

        [DllImport("USER32.dll", CharSet = CharSet.Auto)]
        protected static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [StructLayout(LayoutKind.Sequential)]
        internal struct API_MSG
        {
            public IntPtr Hwnd;
            public int Msg;
            public IntPtr WParam;
            public IntPtr LParam;
            public int Time;
            public POINT Pt;

            public void FromMessage(ref Message msg)
            {
                Hwnd = msg.HWnd;
                Msg = msg.Msg;
                WParam = msg.WParam;
                LParam = msg.LParam;
            }

            public Message ToMessage()
            {
                var message1 = new Message();
                message1.HWnd = Hwnd;
                message1.Msg = Msg;
                message1.WParam = WParam;
                message1.LParam = LParam;
                return message1;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public int message;
            public IntPtr hwnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CWPSTRUCT
        {
            public IntPtr lParam;
            public IntPtr wParam;
            public int message;
            public IntPtr hwnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEHOOKSTRUCT
        {
            public POINT Pt;
            public IntPtr hwnd;
            public uint wHitTestCode;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;
        }
    }

    public class FormHook
    {
    }

    public class ControlWndHookInfo
    {

        public ControlWndHookInfo(ControlWndHook hook, MsgEventHandler handler, MsgEventHandler afterHandler)
        {
            RefCount = 0;
            Hook = hook;
            AddRef(handler, afterHandler);
        }

        public ControlWndHook Hook { get; private set; }

        public int RefCount { get; private set; }

        public void AddRef(MsgEventHandler handler, MsgEventHandler afterHandler)
        {
            RefCount++;
            if (Hook != null)
            {
                if (handler != null)
                {
                    Hook.WndMessage += handler;
                }
                if (afterHandler != null)
                {
                    Hook.AfterWndMessage += afterHandler;
                }
            }
        }

        public void Release(MsgEventHandler handler, MsgEventHandler afterHandler)
        {
            if (Hook != null)
            {
                if (handler != null)
                {
                    Hook.WndMessage -= handler;
                }
                if (afterHandler != null)
                {
                    Hook.AfterWndMessage -= afterHandler;
                }
            }
            if ((--RefCount == 0) && (Hook != null))
            {
                ReleaseCore();
            }
        }

        protected void ReleaseCore()
        {
            Hook.Control = null;
            Hook = null;
        }
    }

    public class ControlWndHook
    {
        private const int GWL_WNDPROC = -4;
        private static readonly Hashtable hooks;
        private Control control;
        private IntPtr prevProc;
        private MyCallBack wndProc;

        static ControlWndHook()
        {
            hooks = new Hashtable();
        }

        public ControlWndHook()
        {
            control = null;
            prevProc = IntPtr.Zero;
        }

        public Control Control
        {
            get { return control; }
            set
            {
                if (Control != value)
                {
                    if (Control != null)
                    {
                        UnHook();
                    }
                    control = value;
                    if (Control != null)
                    {
                        Hook();
                    }
                }
            }
        }

        public static void AddHook(Control ctrl, MsgEventHandler handler, MsgEventHandler afterHandler)
        {
            var info1 = hooks[ctrl] as ControlWndHookInfo;
            if (info1 == null)
            {
                var hook1 = new ControlWndHook();
                hook1.Control = ctrl;
                info1 = new ControlWndHookInfo(hook1, handler, afterHandler);
                hooks[ctrl] = info1;
            }
            else
            {
                info1.AddRef(handler, afterHandler);
            }
        }

        public virtual void Hook()
        {
            UnHook();
            if (Control != null)
            {
                HookCore();
                Control.HandleDestroyed += OnControl_HandleDestroyed;
                Control.HandleCreated += OnControl_HandleCreated;
            }
        }

        protected virtual void HookCore()
        {
            if ((Control != null) && Control.IsHandleCreated)
            {
                wndProc = WindowProc;
                prevProc = SetWindowLong2(new HandleRef(this, Control.Handle), -4, wndProc);
            }
        }

        protected virtual void OnControl_HandleCreated(object sender, EventArgs e)
        {
            Hook();
        }

        protected virtual void OnControl_HandleDestroyed(object sender, EventArgs e)
        {
            UnHook(false);
        }

        public static void RemoveHook(Control ctrl, MsgEventHandler handler, MsgEventHandler afterHandler)
        {
            var info1 = hooks[ctrl] as ControlWndHookInfo;
            if (info1 != null)
            {
                info1.Release(handler, afterHandler);
                if (info1.RefCount == 0)
                {
                    hooks.Remove(ctrl);
                }
            }
        }

        public virtual void UnHook()
        {
            UnHook(true);
        }

        protected virtual void UnHook(bool unsubscribeEvents)
        {
            if (Control != null)
            {
                if (Control.IsHandleCreated && (prevProc != IntPtr.Zero))
                {
                    SetWindowLong(new HandleRef(this, Control.Handle), -4, prevProc);
                }
                wndProc = null;
                prevProc = IntPtr.Zero;
                if (unsubscribeEvents)
                {
                    Control.HandleDestroyed -= OnControl_HandleDestroyed;
                    Control.HandleCreated -= OnControl_HandleCreated;
                }
            }
        }

        public IntPtr WindowProc(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam)
        {
            var message1 = new Message();
            message1.HWnd = hWnd;
            message1.WParam = wParam;
            message1.LParam = lParam;
            message1.Msg = message;
            var ptr1 = prevProc;
            if (WndMessage != null)
            {
                WndMessage(this, ref message1);
            }
            if (ptr1 == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            var ptr2 = CallWindowProc(ptr1, message1.HWnd, message1.Msg, message1.WParam, message1.LParam);
            if (AfterWndMessage != null)
            {
                AfterWndMessage(this, ref message1);
            }
            return ptr2;
        }

        public event MsgEventHandler AfterWndMessage;
        public event MsgEventHandler WndMessage;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CallWindowProc(IntPtr pPrevProc, IntPtr hWnd, int message, IntPtr wParam,
            IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern MyCallBack GetWindowLong(HandleRef hWnd, int nIndex);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLong(HandleRef hWnd, int nIndex, IntPtr newLong);

        [DllImport("User32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLong2(HandleRef hWnd, int nIndex, MyCallBack newLong);

        private delegate IntPtr MyCallBack(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
    }

    #endregion
}
