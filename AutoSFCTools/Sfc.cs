using SFCTools;
using System;
using System.IO.Ports;

namespace AutoSFCTools
{
  public  class Sfc
    {
        private SerialPortImpl sfc;
        private object sfcLock = new object();
        public void Open(int sPort)
        {
            try
            {
                string value = string.Format("{0},{1},{2},{3},{4}", "COM"+sPort, 115200, "None", 8, "One");
                string[] array = value.Split(new char[]
				{
					','
				});
                if (array[0].Contains("COM"))
                {
                    sfc = new SerialPortImpl();
                    if (array.Length > 2)
                    {
                        this.sfc.Open(array[0].Trim(), array[1].Trim(), (Parity)Enum.Parse(typeof(Parity), array[2].Trim()), Convert.ToInt32(array[3].Trim()), (StopBits)Enum.Parse(typeof(StopBits), array[4].Trim()));
                    }
                    else
                    {
                        this.sfc.Open(array[0], array[1]);
                    }
                    System.Threading.Thread.Sleep(50);
                    this.sfc.Close();
                }
            }
            catch (Exception ex)
            {
                var msg = "InitializeSerialport Exception:\r\n" + ex.Message;
                LogHelper.Info(msg);
            }
        }

        public bool GetUnitInfo( string SerialNumber)
        {
            bool flag = false;
            string StationID=StationInfo.StationID.ToUpper();
            string UserID = StationInfo.UserID.ToUpper();
            lock (sfcLock)
            {
                try
                {
                    string unitInfo = "";
                    if (this.sfc != null)
                    {
                        if (!this.sfc.IsOpen())
                        {
                            this.sfc.Open();
                        }
                        StationInfo.FLAG = 0;
                        string sendstr = string.Format("1>>{0},{2},{1},#\r\n", SerialNumber, UserID, StationID);                       
                        sfc.SendData(sendstr);
                        ShowLog.ShowTestLog(sendstr.Trim());
                        if (sendstr.Length>0)
                        {
                            LogHelper.Info(sendstr); 
                        }
                        DateTime t = DateTime.Now.AddMilliseconds((double)5000);
                        while (DateTime.Compare(DateTime.Now, t) <= 0)
                        {
                            unitInfo = sfc.ReceiveData();                           
                            if (unitInfo.Contains("UNIT STATUS IS VALID"))
                            {
                                flag = true;                               
                                ShowLog.ShowTestLog(unitInfo.Trim());
                                if (unitInfo.Length > 0)
                                {
                                    LogHelper.Info(unitInfo);
                                }
                                StationInfo.FLAG = 1;
                                break;
                            }
                            System.Threading.Thread.Sleep(20);
                        }
                        if (!flag)
                        {
                            ShowLog.ShowTestLog(unitInfo.Trim());
                        }
                    }
                }
                  
                catch (Exception ex)
                {
                    var msg = "GetUnitInfo Exception:\r\n" + ex.Message;
                    LogHelper.Info(msg);
                }
            }
            return flag;
        }
        public bool SaveResult(string SerialNumber, string ErrorCode, out string errorMsg)
        {
            bool flag = false;
            string sendstr = "no string";
            errorMsg = "";
            string unitInfo = "";
            lock (sfcLock)
            {
                try
                {
                    if (this.sfc != null)
                    {
                        if (!this.sfc.IsOpen())
                        {
                            this.sfc.Open();
                        }
                        if(ErrorCode=="P")
                        {
                            ErrorCode = "PASS";
                        }
                        sendstr = string.Format("2>>{0};#{1}\r\n", SerialNumber, ErrorCode);
                        sfc.SendData(sendstr);
                        ShowLog.ShowTestLog(sendstr.Trim());
                        if (sendstr.Length > 0)
                        {
                            LogHelper.Info(sendstr);
                        }
                        DateTime t = DateTime.Now.AddMilliseconds((double)5000);
                        while (DateTime.Compare(DateTime.Now, t) <= 0)
                        {
                            unitInfo = sfc.ReceiveData() + "\r\n";                        
                            errorMsg = unitInfo;
                            if (unitInfo.Contains("OK"))
                            {
                                flag = true;
                                ShowLog.ShowTestLog(errorMsg.Trim());
                                if (unitInfo.Length > 0)
                                {
                                    LogHelper.Info(unitInfo);
                                }
                                break;                            
                            }
                            System.Threading.Thread.Sleep(20);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = "SaveResult Exception:\r\n" + ex.Message;
                    LogHelper.Info(msg);
                }
                if (!flag)
                {
                    unitInfo = "no reply";
                }
                //RecordInfoToDB(SerialNumber, sendstr, unitInfo);

            }
            return flag;
        }
        public void Close()
        {
            if (sfc!=null)
            {
                sfc.Close();
            }
           
        }
    }
}
