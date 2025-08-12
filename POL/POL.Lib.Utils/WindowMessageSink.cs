using System;
using System.ComponentModel;

namespace POL.Lib.Utils
{
    public class WindowMessageSink : IDisposable
    {
        #region CreateMessageWindow

        private void CreateMessageWindow()
        {
            WindowId = "WPFTaskbarIcon_" + DateTime.Now.Ticks;

            messageHandler = OnWindowMessageReceived;

            WindowClass wc;

            wc.style = 0;
            wc.lpfnWndProc = messageHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = "";
            wc.lpszClassName = WindowId;

            HelperUser32.RegisterClass(ref wc);

            taskbarRestartMessageId = HelperUser32.RegisterWindowMessage("TaskbarCreated");

            MessageWindowHandle = HelperUser32.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, 0, 0, 0, 0);

            if (MessageWindowHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }

        #endregion

        #region members

        public const int CallbackMessageId = 0x400;

        private bool isDoubleClick;

        private WindowProcedureHandler messageHandler;

        private uint taskbarRestartMessageId;

        internal string WindowId { get; private set; }

        public IntPtr MessageWindowHandle { get; private set; }

        public EnumNotifyIconVersion Version { get; set; }

        #endregion

        #region events

        public event Action<bool> ChangeToolTipStateRequest;

        public event Action<EventMouseEvent> MouseEventReceived;

        public event Action<bool> BalloonToolTipChanged;

        public event Action TaskbarCreated;

        #endregion

        #region construction

        public WindowMessageSink(EnumNotifyIconVersion version)
        {
            Version = version;
            CreateMessageWindow();
        }


        private WindowMessageSink()
        {
        }


        public static WindowMessageSink CreateEmpty()
        {
            return new WindowMessageSink
            {
                MessageWindowHandle = IntPtr.Zero,
                Version = EnumNotifyIconVersion.Vista
            };
        }

        #endregion

        #region Handle Window Messages

        private long OnWindowMessageReceived(IntPtr hwnd, uint messageId, uint wparam, uint lparam)
        {
            if (messageId == taskbarRestartMessageId)
            {
                if (TaskbarCreated != null) TaskbarCreated();
            }

            ProcessWindowMessage(messageId, wparam, lparam);

            return HelperUser32.DefWindowProc(hwnd, messageId, wparam, lparam);
        }


        private void ProcessWindowMessage(uint msg, uint wParam, uint lParam)
        {
            if (msg != CallbackMessageId) return;

            switch (lParam)
            {
                case 0x200:
                    MouseEventReceived(EventMouseEvent.MouseMove);
                    break;

                case 0x201:
                    MouseEventReceived(EventMouseEvent.IconLeftMouseDown);
                    break;

                case 0x202:
                    if (!isDoubleClick)
                    {
                        MouseEventReceived(EventMouseEvent.IconLeftMouseUp);
                    }
                    isDoubleClick = false;
                    break;

                case 0x203:
                    isDoubleClick = true;
                    MouseEventReceived(EventMouseEvent.IconDoubleClick);
                    break;

                case 0x204:
                    MouseEventReceived(EventMouseEvent.IconRightMouseDown);
                    break;

                case 0x205:
                    MouseEventReceived(EventMouseEvent.IconRightMouseUp);
                    break;

                case 0x206:
                    break;

                case 0x207:
                    MouseEventReceived(EventMouseEvent.IconMiddleMouseDown);
                    break;

                case 520:
                    MouseEventReceived(EventMouseEvent.IconMiddleMouseUp);
                    break;

                case 0x209:
                    break;

                case 0x402:
                    BalloonToolTipChanged(true);
                    break;

                case 0x403:
                case 0x404:
                    BalloonToolTipChanged(false);
                    break;

                case 0x405:
                    MouseEventReceived(EventMouseEvent.BalloonToolTipClicked);
                    break;

                case 0x406:
                    ChangeToolTipStateRequest(true);
                    break;

                case 0x407:
                    ChangeToolTipStateRequest(false);
                    break;
            }
        }

        #endregion

        #region Dispose

        public bool IsDisposed { get; private set; }


        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~WindowMessageSink()
        {
            Dispose(false);
        }


        private void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;
            IsDisposed = true;

            HelperUser32.DestroyWindow(MessageWindowHandle);
            messageHandler = null;
        }

        #endregion
    }
}
