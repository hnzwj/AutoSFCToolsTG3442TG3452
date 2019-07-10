using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using TestTools;

namespace AutoSFCTools
{
    public enum eTransportType
    {
        telnet = 0,
        SSH = 1
    }

    public enum WifiDeviceType
    {
        Litepoint = 0,
        NI = 1,
        Undefine = 2
    }

    public enum LoginType
    {
        Arm = 0,
        Atom = 1,
        AtomWithPass = 2,
        Undefine = 3
    }
    public class CommImpl
    {
        private string strCmdLine = "";
        //private eTransportType protocol;
        private IPAddress localHost;
        private IPAddress remoteHost;
        private long bindingAddress;
        private int TimeoutMS = 10000;
        private Process cmdProcess;
        private StreamWriter sortStreamWriter;
        private StringBuilder returnData;
        private string _strCmd;
        private string PlinkFileName;
       
        public CommImpl(IPAddress Local, IPAddress Remote, string ProcessName, int Socket)
        {
            //protocol = ETransportType;
            localHost = Local;
            remoteHost = Remote;
            PlinkFileName = ProcessName;
            this.returnData = new StringBuilder("");
            this.returnData.EnsureCapacity(1024);
        }
        public void open()
        {
            GetCmdLine();
            this.cmdProcess = new Process();
            //this.cmdProcess.StartInfo.FileName = "C:\\mtp\\bin\\" + PlinkFileName;
            this.cmdProcess.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\" + PlinkFileName;
            //if (!string.IsNullOrEmpty(dir))
            //{
            //    p.StartInfo.WorkingDirectory = dir;
            //}
            this.cmdProcess.StartInfo.Arguments = string.Format(strCmdLine);
            this.cmdProcess.StartInfo.UseShellExecute = false;
            this.cmdProcess.StartInfo.CreateNoWindow = true;
            this.cmdProcess.StartInfo.RedirectStandardOutput = true;
            this.cmdProcess.StartInfo.RedirectStandardError = true;
            this.cmdProcess.StartInfo.RedirectStandardInput = true;
            this.returnData.Remove(0, this.returnData.Length);
            this.cmdProcess.OutputDataReceived += new DataReceivedEventHandler(this.OutputHandler);
            this.cmdProcess.Start();
            this.sortStreamWriter = this.cmdProcess.StandardInput;
            this.cmdProcess.BeginOutputReadLine();
        }
        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                if (outLine.Data.Contains("::~"))
                {
                    this.returnData.Append(Environment.NewLine);
                }
                this.returnData.Append(outLine.Data + Environment.NewLine);
            }
        }
        public string read(int DelayTimeMS)
        {
            Thread.Sleep(DelayTimeMS);
            string strResponse = this.returnData.ToString();
            bool flag = this._strCmd != null && this._strCmd.Length > 0 && strResponse.Contains(this._strCmd);
            if (flag)
            {
                int num = strResponse.IndexOf(this._strCmd);
                if (num >= 0)
                {
                    strResponse = strResponse.Substring(num + _strCmd.Length);
                }
            }
            strResponse = strResponse.Replace("/bin/sh: ?: not found", "");
            this.returnData.Remove(0, this.returnData.Length);
           ShowLog.ShowTestLog(string.Format("UUT Read = {0}", strResponse));
            return strResponse;
        }
        public int read(ref string strResponse, double timeout)
        {
            DateTime t = DateTime.Now.AddMilliseconds((double)timeout);

            int result = 99;
            bool flag = false;
            string supplement = "";
            if (strResponse.Length > 0)
            {
                supplement = strResponse;
            }
            else
            {
                supplement = "Return Status: 0";
            }
            while (!flag && DateTime.Compare(DateTime.Now, t) <= 0)
            {
                strResponse = this.returnData.ToString();

                if (this._strCmd != null && this._strCmd.Length > 0 && strResponse.Contains(this._strCmd))
                {
                    int num = strResponse.IndexOf(this._strCmd);
                    if (num >= 0)
                    {
                        strResponse = strResponse.Substring(num + _strCmd.Length);
                    }
                }

                if (strResponse.Contains(supplement))
                {
                    flag = true;
                    result = 0;
                }
                Thread.Sleep(50);
            }

            strResponse = strResponse.Replace("/bin/sh: ?: not found", "");
            ShowLog.ShowTestLog(string.Format("UUT Read = {0}", strResponse));
            this.returnData.Remove(0, this.returnData.Length);
            return result;
        }

        public bool read(string[] Expected, double timeout, out string ReturnData)
        {
            bool flag = false;
            string reply = "";
            ReturnData = "";
            DateTime t = DateTime.Now.AddMilliseconds((double)timeout);
            while (!flag && DateTime.Compare(DateTime.Now, t) <= 0)
            {
                reply = this.returnData.ToString();
                if (this._strCmd != null && this._strCmd.Length > 0 && reply.Contains(this._strCmd))
                {
                    int num = reply.IndexOf(this._strCmd);
                    if (num >= 0)
                    {
                        reply = reply.Remove(0, num + _strCmd.Length);
                    }
                }
                if (reply.Length > 0)
                {
                    foreach (string str in Expected)
                    {
                        if (reply.Contains(str))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                Thread.Sleep(50);
            }
            reply = reply.Replace("/bin/sh: ?: not found", "");
            ShowLog.ShowTestLog(string.Format("UUT Read = {0}", reply));
            ReturnData = reply;
            this.returnData.Remove(0, this.returnData.Length);
            return flag;
        }
        public int write(string strSend)
        {
            this.returnData.Remove(0, this.returnData.Length);
            Thread.Sleep(100);
            this.returnData.Remove(0, this.returnData.Length);
            if (null == sortStreamWriter)
            {
                return -2;
            }
            bool canWrite = this.sortStreamWriter.BaseStream.CanWrite;
            if (canWrite)
            {
                this._strCmd = strSend;
                if (!strSend.Contains("\r"))
                {
                    sortStreamWriter.Write(strSend + System.Environment.NewLine);
                }
                else
                {
                    sortStreamWriter.Write(strSend);
                }
            
                ShowLog.ShowTestLog(string.Format("Write = {0}", strSend));
            }
            return 0;
        }
        private int GetCmdLine()
        {

            string[] array = this.localHost.ToString().Split(new char[] { '.' });
            long num = Convert.ToInt64(array[0]) * 256L;
            long num2 = (Convert.ToInt64(array[1]) + num) * 256L;
            long num3 = (Convert.ToInt64(array[2]) + num2) * 256L;
            this.bindingAddress = Convert.ToInt64(array[3]) + num3;
            this.strCmdLine = string.Format("  -{0} {1} -B {2}", new object[]
            {
                "telnet",
                this.remoteHost,
                this.bindingAddress
            });
            return 0;
        }
        public void Close()
        {
            if (null != cmdProcess && !cmdProcess.HasExited)
            {
                cmdProcess.Kill();
            }
            sDelay.Delay(1000);
            killprocess(PlinkFileName);
        }
        private void killprocess(string proceeName = "cmd")
        {
            try
            {
                foreach (Process thisproc in Process.GetProcesses())
                {
                    if (thisproc.ProcessName == proceeName)
                    {
                        thisproc.Kill();
                        //this.log.Debug("Kill " + proceeName + " pass");
                        sDelay.Delay(500);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //this.log.Error("Kill " + proceeName + " fail" + ex.Message);
            }
        }
    }
}
