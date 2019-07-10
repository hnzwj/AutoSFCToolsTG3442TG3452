using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AutoSFCTools
{
    class win32API
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        const int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;// 模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标 

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);//dwFlags:0表示按下，2表示弹起
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int capacity, string text);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标 
        }
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        public delegate bool CallBack(IntPtr hwnd, int IParam);
        [DllImport("user32.dll")]
        public static extern int EnumWindows(CallBack x, int y);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        public IntPtr GetDesktopWindows(string windowname)
        {
            List<WindowsInfo> wndList = new List<WindowsInfo>();
            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                WindowsInfo wnd = new WindowsInfo();
                StringBuilder sb = new StringBuilder(256);
                wnd.wWnd = hWnd;
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowname = sb.ToString();

                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                wndList.Add(wnd);
                return true;

            }, 0);
            // return wndList.ToArray();
            foreach (var windowinfo in wndList.ToArray())
            {
                if (windowinfo.szWindowname.Contains(windowname))
                {
                    return windowinfo.wWnd;
                }
            }
            return (IntPtr)(-1);
        }
        public void MoveMouse(IntPtr windowsHandle, double xPercent, double yPercent)
        {
            RECT rect = new RECT();
            GetWindowRect(windowsHandle, ref rect);
            int xLeftTop = rect.Left;                     //左上角坐标
            int yLeftTop = rect.Top;

            int xRightTop = rect.Right;                 //右上角坐标
            int yRightTop = rect.Top;

            //int height = Screen.PrimaryScreen.Bounds.Height;
            //int width = Screen.PrimaryScreen.Bounds.Width;

            //mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (xLeftTop + (int)((xRightTop - xLeftTop) * xPercent)) * 65536 / width, (yLeftTop + y) * 65536 / height, 0, 0);//ARFTS y=190
            //// mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (xLeftTop + x) * 65536 / width, (yLeftTop +y) * 65536 / height, 0, 0);
            //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            int Bottom = rect.Bottom;

            int height = Screen.PrimaryScreen.Bounds.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;


            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (xLeftTop + (int)((xRightTop - xLeftTop) * xPercent)) * 65536 / width, ((int)((Bottom - yLeftTop) * yPercent) + yLeftTop) * 65536 / height, 0, 0);//ARFTS y=190
            // mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (xLeftTop + x) * 65536 / width, (yLeftTop +y) * 65536 / height, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        public void MoveMouse(IntPtr windowsHandle, int x, int y)//double yPercent)
        {
            RECT rect = new RECT();
            GetWindowRect(windowsHandle, ref rect);
            int xLeftTop = rect.Left;                     //左上角坐标
            int yLeftTop = rect.Top;
            int xRightTop = rect.Right;                 //右上角坐标
            int yRightTop = rect.Top;
            int height = Screen.PrimaryScreen.Bounds.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;
            // mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (xLeftTop + (int)((xRightTop - xLeftTop) * xPercent)) * 65536 / width, (yLeftTop + 190) * 65536 / height, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (xLeftTop + x) * 65536 / width, (yLeftTop + y) * 65536 / height, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }


















        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);
        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        //[StructLayout(LayoutKind.Sequential)]
        //public struct RECT
        //{
        //    int left;
        //    int top;
        //    int right;
        //    int bottom;
        //}
        public GUITHREADINFO? GetGuiThreadInfo(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                uint threadId = GetWindowThreadProcessId(hwnd, IntPtr.Zero);
                GUITHREADINFO guiThreadInfo = new GUITHREADINFO();
                guiThreadInfo.cbSize = Marshal.SizeOf(guiThreadInfo);
                if (GetGUIThreadInfo(threadId, ref guiThreadInfo) == false)
                    return null;
                return guiThreadInfo;
            }
            return null;
        }
        public void SendText(string text)
        {
            IntPtr hwnd = GetForegroundWindow();
            if (String.IsNullOrEmpty(text))
                return;
            GUITHREADINFO? guiInfo = GetGuiThreadInfo(hwnd);
            if (guiInfo != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    //hwndFocus字面意思就是当前光标处的句柄
                    SendMessage(guiInfo.Value.hwndFocus, 0x0102, (IntPtr)(int)text[i], IntPtr.Zero);
                }
            }
        }
    }
    public class WindowsInfo
    {
        public IntPtr wWnd;
        public string szWindowname;
        public string szClassName;
    }
}
