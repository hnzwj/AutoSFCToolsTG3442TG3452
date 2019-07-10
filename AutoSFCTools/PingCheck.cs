using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using TestTools;

namespace AutoSFCTools
{
    class PingCheck
    {
        ManualResetEvent _mre;
        private bool _result = false;
        private string _localhost = string.Empty;
        private double _timeOutMs = 0;
        private DateTime startTime;
        public bool Result
        {
            get { return this._result; }
        }
        public string LocalHost
        {
            set { this._localhost = value; }
        }
        public double TimeOutMs
        {
            set { this._timeOutMs = value; }
        }
        public PingCheck(ManualResetEvent mre)
        {
            _mre = mre;
        }
        public PingCheck()
        {
           
        }
        
        public void PingTest()
        {
            CmdHelper cmd = new CmdHelper();
            bool flag = false;
            this.startTime = DateTime.Now;
            try
            {
                string scmd = string.Format("ping -n 1 {0} -S {1} -w 300", "192.168.100.1", _localhost);
                string sRedata = "";
                DateTime t = DateTime.Now.AddMilliseconds((double)_timeOutMs);
                while (!flag && DateTime.Compare(DateTime.Now, t) <= 0)
                {
                    cmd.RunCmd(scmd, out sRedata);
                    ShowLog.ShowTestLog(sRedata.Replace(scmd,""));
                    if (sRedata.Contains("bytes=32") || sRedata.Contains("字节=32"))
                    {
                        flag = true;
                        _result = true;
                        break;
                    }
                    sDelay.Delay(200);
                }
                //_mre.Set();
            }
            catch (Exception ex)
            {
               ShowLog.ShowErrorLog(string.Format("Error {0})", ex.Message));
            }
            finally
            {
                ShowLog.ShowTestLog(string.Format("Ping Test time:{0}", ((TimeSpan)(DateTime.Now - startTime)).TotalSeconds.ToString("00.00")));
            }       
        }

        public int NetworkPing(string IPaddress, int TimeoutMs)
        {
            int num = -99;
            DateTime t = DateTime.Now.AddMilliseconds((double)TimeoutMs);
            try
            {
                //GetNetworkInterfaceInfo();
                Ping ping = new Ping();
                PingOptions options = new PingOptions();
                string s = "test data";
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                bool flag = false;
                while (!flag && DateTime.Compare(DateTime.Now, t) <= 0)
                {
                    PingReply pingReply = ping.Send(IPaddress, 1000, bytes, options);
                    if (IPStatus.Success== pingReply.Status)
                    {
                        flag = true;
                        num = 0;
                    }
                }
                ShowLog.ShowTestLog(string.Format("Try to ping target host resutl is {0}", new object[] { num })); ;
            }
            catch (TimeoutException ex)
            {
                ShowLog.ShowTestLog(string.Format("NetworkPing timeout. {0}", ex.Message));
            }
            catch (Exception ex)
            {
                ShowLog.ShowTestLog(string.Format("NetworkPing Exception. {0}", ex.Message));
            }
            return num;
        }

        /// <summary>
        /// 判断网络是否Ping通
        /// </summary>
        /// <param name="TimeOutMs">毫秒</param>
        /// <param name="remoteIP"></param>
        /// <param name="localIP"></param>
        /// <returns></returns>
        public bool NetworkPing(double TimeOutMs, string remoteIP, string localIP)
        {
            this.startTime = DateTime.Now;
            CmdHelper cmd = new CmdHelper();
            bool flag = false;
            try
            {
                string scmd = string.Format("ping -n 1 {0} -S {1} -w 500", remoteIP, localIP);
                string sRedata = "";
                DateTime t = DateTime.Now.AddMilliseconds((double)TimeOutMs);
                while (!flag && DateTime.Compare(DateTime.Now, t) <= 0)
                {
                    cmd.RunCmd(scmd, out sRedata);
                    if (sRedata.Contains("bytes=32") || sRedata.Contains("字节=32"))
                    {
                        flag = true;
                        break;
                    }
                    sDelay.Delay(200);
                }
            }
            catch (Exception ex)
            {
                ShowLog.ShowErrorLog(string.Format("Error {0})", ex.Message));
            }
            finally
            {
                ShowLog.ShowTestLog(string.Format("Ping Test time:{0}", ((TimeSpan)(DateTime.Now - startTime)).TotalSeconds.ToString("00.00")));
            }
            return flag;
        }

        /// <summary>
        /// 判断网络是否Ping通
        /// </summary>
        /// <param name="TimeOutMs">毫秒</param>
        /// <param name="remoteIP"></param>
        /// <param name="localIP"></param>
        /// <returns></returns>
        public bool NetworkPing(double TimeOutMs, string remoteIP)
        {
            this.startTime = DateTime.Now;
            CmdHelper cmd = new CmdHelper();
            bool flag = false;
            try
            {
                string scmd = string.Format("ping -n 1 {0}  -w 500", remoteIP);
                string sRedata = "";
                DateTime t = DateTime.Now.AddMilliseconds((double)TimeOutMs);
                while (!flag && DateTime.Compare(DateTime.Now, t) <= 0)
                {
                    cmd.RunCmd(scmd, out sRedata);
                    if (sRedata.Contains("bytes=32") || sRedata.Contains("字节=32"))
                    {
                        flag = true;
                        break;
                    }
                    sDelay.Delay(200);
                }
            }
            catch (Exception ex)
            {
                ShowLog.ShowErrorLog(string.Format("Error {0})", ex.Message));
            }
            finally
            {
                ShowLog.ShowTestLog(string.Format("Ping Test time:{0}", ((TimeSpan)(DateTime.Now - startTime)).TotalSeconds.ToString("00.00")));
            }
            return flag;
        }
    }
}
