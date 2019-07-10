using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoSFCTools
{
    public partial class Debug : Form
    {
        public Debug()
        {
            InitializeComponent();
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

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            int left;
            int top;
            int right;
            int bottom;
        }
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
        protected void SendText(string text)
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











        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        //定义钩子句柄
        public static int hHook = 0;
        //定义钩子类型
        public const int WH_MOUSE_LL = 14;
        public HookProc MyProcedure;
        //安装钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        //调用下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }






        string orgPath = @"C:\afts.bmp";
        string tempPath = @"C:\temp.bmp";
        private void Debug_Load(object sender, EventArgs e)
        {
            pictureBox1.Image =Bitmap.FromFile( orgPath);
            pictureBox3.Image = Bitmap.FromFile(tempPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
             Bitmap sourceBitmap = new Bitmap(orgPath);
             Bitmap TempBitmap = new Bitmap(tempPath);
             //timer1.Enabled = true;
        }
        Point point = new Point();
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;


            if (Cursor == System.Windows.Forms.Cursors.IBeam)
            {
                label1.Text = "Input";
                
            }
            else
            {
                label1.Text = "Not Input";
            }
            point.Y = Cursor.Position.Y - this.Location.Y;
            point.X = Cursor.Position.X - this.Location.X;
            label2.Text = point.X.ToString();
            label3.Text = point.Y.ToString();



            Point ms = Control.MousePosition;
            this.label2.Text = string.Format("{0}:{1}", ms.X, ms.Y);
            MouseButtons mb = Control.MouseButtons;
          
            if (mb == System.Windows.Forms.MouseButtons.Left) this.label3.Text = "Left";
            if (mb == System.Windows.Forms.MouseButtons.Right) this.label3.Text = "Right";
            if (mb == System.Windows.Forms.MouseButtons.Middle) this.label3.Text = "Middle";


            SendText("123\r");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (hHook == 0)
            {
                MyProcedure = new HookProc(this.MouseHookProc);
                //这里挂节钩子
                //C#实现的鼠标钩子，可以获取鼠标在屏幕中的坐标，记得要以管理员权限运行才行
                hHook = SetWindowsHookEx(WH_MOUSE_LL, MyProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (hHook == 0)
                {
                    MessageBox.Show("SetWindowsHookEx Failed");
                    return;
                }
                button1.Text = "卸载钩子";
            }
            else
            {
                bool ret = UnhookWindowsHookEx(hHook);
                if (ret == false)
                {
                    MessageBox.Show("UnhookWindowsHookEx Failed");
                    return;
                }
                hHook = 0;
                button1.Text = "安装钩子";
            }
        }
        public int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {

            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            if (nCode < 0)
            {
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                String strCaption = "x = " + MyMouseHookStruct.pt.x.ToString("d") + "  y = " + MyMouseHookStruct.pt.y.ToString("d");
                this.Text = strCaption;
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        //private bool isTrigger(Key[] combKeys)
        //{
        //    //获取程序句柄
        //    IntPtr hWnd = User32.GetForegroundWindow();
        //    uint processId = 0;
        //    //获取线程号
        //    uint threadid = User32.GetWindowThreadProcessId(hWnd, ref processId);
        //    //GUI信息
        //    GUITHREADINFO lpgui = new GUITHREADINFO();
        //    lpgui.cbSize = Marshal.SizeOf(lpgui);

        //    //判断当前进程是否存在光标
        //    if (User32.GetGUIThreadInfo(threadid, ref lpgui))
        //    {
        //        if (lpgui.hwndCaret != 0)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

    }
}
