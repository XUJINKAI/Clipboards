using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace WpfBackground
{
    static class AppHelper
    {
        private static Mutex mutex;
        private static bool isNewInstance;

        public static bool IsNewInstance(string AppId)
        {
            if (mutex == null)
            {
                mutex = new Mutex(true, AppId, out isNewInstance);
                return isNewInstance;
            }
            else
            {
                return isNewInstance;
            }
        }

        public static void BroadcastMessage(string msg)
        {
            NativeMethods.PostMessage(
                (IntPtr)NativeMethods.HWND_BROADCAST,
                GetMesssageId(msg),
                IntPtr.Zero,
                IntPtr.Zero);
        }

        private static readonly Dictionary<string, int> RegisteredMessage = new Dictionary<string, int>();
        private static readonly Dictionary<int, Action> MsgActionDict = new Dictionary<int, Action>();
        public static bool AutoRestart { get; set; } = false;
        private static string AutoRestartParameter = "";
        private static Action AutoRestartShutdownAction;

        private static int GetMesssageId(string msg)
        {
            if (RegisteredMessage.ContainsKey(msg))
            {
                return RegisteredMessage[msg];
            }
            else
            {
                int msgid = NativeMethods.RegisterWindowMessage(msg);
                RegisteredMessage[msg] = msgid;
                return msgid;
            }
        }

        public static void RegisterReciveMessage(string msg, Action action)
        {
            int msgid = GetMesssageId(msg);
            MsgActionDict[msgid] = action;
        }

        public static void RegisterAutoRestart(Action shutdownAction, string Parameter = "")
        {
            AutoRestartShutdownAction = shutdownAction;
            AutoRestartParameter = Parameter;
            AutoRestart = true;
        }

        private static AppHelperWindow _window;

        static AppHelper()
        {
            _window = new AppHelperWindow();
            _window.Show();
            AppHelperWindow.RecivingMsg += AppHelperWindow_RecivingMsg;
        }

        private static void AppHelperWindow_RecivingMsg(int msg)
        {
            if (AutoRestart)
            {
                if (msg == NativeMethods.WM_QUERYENDSESSION)
                {
                    NativeMethods.RegisterApplicationRestart(AutoRestartParameter, 0);
                }
                if (msg == NativeMethods.WM_ENDSESSION)
                {
                    AutoRestartShutdownAction();
                }
            }
            if (MsgActionDict.ContainsKey(msg))
            {
                Action action = MsgActionDict[msg];
                action();
            }
        }
    }

    static class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public const UInt32 WM_QUERYENDSESSION = 0x0011;
        public const UInt32 WM_ENDSESSION = 0x0016;

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int RegisterApplicationRestart([MarshalAs(UnmanagedType.LPWStr)] string commandLineArgs, int Flags);
    }

    class AppHelperWindow : Window
    {
        public static event Action<int> RecivingMsg;

        public AppHelperWindow()
        {
            Visibility = Visibility.Hidden;
            Opacity = 0;
            Top = 0;
            Left = 0;
            Width = 0;
            Height = 0;
            WindowStyle = WindowStyle.ToolWindow;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(new HwndSourceHook(WndProc));

            Visibility = Visibility.Hidden;
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            RecivingMsg?.Invoke(msg);
            return IntPtr.Zero;
        }

    }
}
