using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSFCTools
{
    public partial class debug2 : Form
    {
        public debug2()
        {
            InitializeComponent();
        }



        #region 得到光标在屏幕上的位置
        [DllImport("user32")]
        public static extern bool GetCaretPos(out Point lpPoint);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        private static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern void ClientToScreen(IntPtr hWnd, ref Point p);

        private Point CaretPos()
        {
            IntPtr ptr = GetForegroundWindow();
            Point p = new Point();

            //得到Caret在屏幕上的位置   
            if (ptr.ToInt32() != 0)
            {
                IntPtr targetThreadID = GetWindowThreadProcessId(ptr, IntPtr.Zero);
                IntPtr localThreadID = GetCurrentThreadId();

                if (localThreadID != targetThreadID)
                {
                    AttachThreadInput(localThreadID, targetThreadID, 1);
                    ptr = GetFocus();
                    if (ptr.ToInt32() != 0)
                    {
                        GetCaretPos(out   p);
                        ClientToScreen(ptr, ref   p);
                    }
                    AttachThreadInput(localThreadID, targetThreadID, 0);
                }
            }
            return p;
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            CaretPos();
        }
    }
}
