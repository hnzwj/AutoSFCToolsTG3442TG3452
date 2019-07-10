using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCTools
{
    class TimeOut
    {
        private readonly int TimeoutInterval = 150000;//设定超时间隔为1000ms
        public long LastTicks;//用于存储新建操作开始的时间
        public long elapsedTicks;//用于存储操作消耗的时间

        public TimeOut()
        {
            LastTicks = DateTime.Now.Ticks;
        }

        public bool IsTimeout()
        {
            elapsedTicks = DateTime.Now.Ticks - LastTicks;
            TimeSpan span = new TimeSpan(elapsedTicks);
            double diff = span.TotalSeconds;
            if (diff > TimeoutInterval)
                return true;
            else
                return false;
        }

    }
}
