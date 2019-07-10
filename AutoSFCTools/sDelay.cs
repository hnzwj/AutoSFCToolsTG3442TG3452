using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TestTools
{
    class sDelay
    {
        [DllImport("kernel32.dll")]
        static extern uint GetTickCount();
        public static void Delay(uint ms)
        {
            uint start = GetTickCount();
            while (GetTickCount() - start < ms)
                Application.DoEvents();

        }
    }
}
