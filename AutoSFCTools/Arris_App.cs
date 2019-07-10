using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TestTools;
using System.Linq;
using System.Threading;
using System.Collections;
using SFCTools;

namespace AutoSFCTools
{
    class Arris_App
    {
       
        public static int Arris_App_RunTest(object group, out Dictionary<string, string> TestInfo)
        {
           
            int errorCode = 99;
            string errorMsg = "";
            DateTime startTime = DateTime.Now;
            string logPath = string.Empty;          
            TestInfo = new Dictionary<string, string>();
            win32API window = new win32API();
            dealLogs deallog =new dealLogs();
            //string logPath = StationInfo.TelnetLogPath;

            if (StationInfo.StationID == "ARFTS" || StationInfo.StationID == "AFTSII")
            {
                logPath = StationInfo.TelnetLogPath;
            }
            if (StationInfo.StationID == "FTTS")
            {
                logPath = StationInfo.ResultLogPath;
            }
            try
            {
                if (Directory.Exists(logPath))
                {
                    FileInfo[] fl = new GetFileList().GetFiles(logPath.ToString());
                    foreach (FileInfo f in fl)
                    {
                        File.Delete(f.FullName);
                    }
                }
                IntPtr mainHandle = SetWindowSize();
                if (mainHandle != IntPtr.Zero)
                {
                    EnterGrop(group.ToString());
                    sDelay.Delay(1000);
                    if (JudgeIsTestStart(200000))
                    {
                        ShowLog.ShowTestLog("Start customer program testing pass");
                    }
                    else
                    {
                        ShowLog.ShowTestLog("Start customer program testing fail");
                        return -1;
                    }
                }
                else
                {
                    ShowLog.ShowTestLog("can not found customer program");
                    return -1;
                }

                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.AddMilliseconds(200000.0);
                bool found = false;
                while (!found && dateTime.CompareTo(DateTime.Now) > 0)
                {
                    if(StationInfo.StationID=="ARFTS"||StationInfo.StationID=="AFTSII")
                    {
                        if (Directory.Exists(logPath))
                        {
                            FileInfo[] fl = new GetFileList().GetFiles(logPath.ToString());
                            foreach (FileInfo f in fl)
                            {
                                if (f.Name.Contains("CPK_") || f.Name.Contains("Line"))//测试完毕
                                {
                                    ShowLog.ShowTestLog(string.Format("Found {0} file.", new object[] { f.Name }));
                                    found = true;
                                    AnaylzerLog(f.FullName, out TestInfo);
                                    errorCode = 0;
                                    break;
                                }
                            }
                        }
                    }
                    else  if(StationInfo.StationID=="FTTS")
                    {
                        if (Directory.Exists(logPath))
                        {
                            FileInfo[] fl = new GetFileList().GetFiles(StationInfo.ResultLogPath.ToString());
                            foreach (FileInfo f in fl)
                            {
                                if (f.Name.Contains("CPK_") || f.Name.Contains("Line"))//测试完毕
                                {
                                    ShowLog.ShowTestLog(string.Format("Found {0} file.", new object[] { f.Name }));
                                    found = true;
                                    AnaylzerLog(f.FullName, out TestInfo);
                                    errorCode = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("Exception:{0}", ex.Message);
                errorCode = -1;
                ShowLog.ShowErrorLog(string.Format("Error {0}: {1})", errorCode, errorMsg));
            }
            finally
            {

            }
            return errorCode;
        }


       public static void AnaylzerLog(string FilePath, out Dictionary<string, string> TestInfo)
        {
            TestInfo = new Dictionary<string, string>();
            try
            {           
                string strLine;
                string keyValue = string.Empty;
                string testTime = DateTime.Now.ToString();
                string time = string.Empty;
                ArrayList arrylist = new ArrayList();

                if (StationInfo.StationID == "ARFTS")
                {
                    StreamReader streamReader = new StreamReader(FilePath);
                    while ((strLine = streamReader.ReadLine()) != null)
                    {
                        arrylist.Add(strLine);
                    }
                    int uut = 0;
                    int b = arrylist.Count;
                    for (int a = b; a > StationInfo.LogCount; a--)
                    {
                        string loginfo = arrylist[a - 1].ToString();
                        string[] list = loginfo.Split('\t');
                        if (list.Length == 6)
                        {
                            string sn = list[1];  //测试无SN的情况
                            if (sn.Length < 1)
                            {
                                continue;
                            }
                            string result = list[5];
                            string errorcode = list[4];
                            keyValue = list[2];
                            if (errorcode.Length < 1)
                            {
                                errorcode = result;
                            }
                            string daytime = DateTime.Now.ToShortDateString().ToString();
                            string Testres = string.Empty;
                            FileInfo[] fl = new GetFileList().GetFiles(StationInfo.TestReport.ToString());
                            foreach (FileInfo f in fl)
                            {
                                if (f.Name.Contains(daytime) && f.Length > 0)
                                {
                                    ReadTestReport(f.FullName, sn, out Testres);
                                    if (Testres == result)
                                    {
                                        //ShowLog.ShowTestLog(string.Format("Get UUT SN = {0} Result = {1} Error = {2}", sn, result, errorcode));
                                        TestInfo.Add(keyValue + "=" + sn, errorcode);
                                    }
                                }
                            }
                        }
                    }
                    streamReader.Close();
                    uut = b - StationInfo.LogCount;
                    if (uut > 0)
                    {
                        StationInfo.LogCount = b;
                    }
                }
                 if (StationInfo.StationID == "AFTSII")
                 {
                     StreamReader streamReader = new StreamReader(FilePath);
                     while ((strLine = streamReader.ReadLine()) != null)
                     {
                         arrylist.Add(strLine);
                     }
                     int b = arrylist.Count;
                     string lineEnd = arrylist[b - 1].ToString();
                     string[] txt = lineEnd.Split('\t');
                     time = txt[0];
                     for (int a = arrylist.Count; a > 0; a--)
                     {
                         string loginfo = arrylist[a - 1].ToString();
                         if (loginfo.Contains(time))
                         {
                             string[] txts = loginfo.Split('\t');
                             if (txts.Length == 9)
                             {
                                 string sn = txts[3];
                                 if (sn.Length < 1)
                                 {
                                     continue;
                                 }
                                 string result = txts[8];
                                 string errorcode = txts[7];
                                 keyValue = txts[5];
                                 if (errorcode.Length < 1)
                                 {
                                     errorcode = result;
                                 }
                                 string daytime = DateTime.Now.ToShortDateString().ToString();
                                 string Testres = string.Empty;
                                 FileInfo[] fl = new GetFileList().GetFiles(StationInfo.TestReport.ToString());
                                 foreach (FileInfo f in fl)
                                 {
                                     if (f.Name.Contains(daytime) & f.Length > 0)
                                     {
                                         ReadTestReport(f.FullName, sn, out Testres);
                                         if (Testres == result)
                                         {
                                             TestInfo.Add(keyValue + "=" + sn, errorcode);
                                         }
                                     }
                                 }
                             }
                         }
                     }
                     streamReader.Close();
                 }                               
                if (StationInfo.StationID == "FTTS")
                {
                    string sn = "";
                    string errorcode = "";
                    string result = "";
                    string res = "";
                    string StrLine = string.Empty;
                    StreamReader SR = new StreamReader(FilePath);
                    while ((StrLine = SR.ReadLine()) != null)
                    {
                        arrylist.Add(StrLine);
                    }
                  
                    string lineEnd = arrylist[arrylist.Count - 1].ToString();
                    string[] txt = lineEnd.Split('\t');
                    time = txt[2];
                    for (int a = arrylist.Count; a > 0; a--)
                    {
                        string loginfo = arrylist[a - 1].ToString();
                        if (loginfo.Contains(time))
                        {
                            string[] list = loginfo.Split('\t');
                            sn = list[3];
                            if (sn.Length < 1)
                            {
                                continue;
                            }
                            res = list[101];
                            if(res=="P")
                            {
                                errorcode = res;
                                result = "PASS";
                            }
                            else if(res!="P")
                            {
                                errorcode = list[106];
                                result = "FAIL";
                            }                              
                           // ShowLog.ShowTestLog(string.Format("Get UUT SN = {0} Result = {1} Error = {2}", sn, result, errorcode));
                            TestInfo.Add(a + "=" + sn, errorcode);
                        }
                        else
                        {
                            break;
                        }
                    }
                    //if (sn != "")
                    //{
                    //    ShowLog.ShowTestLog(string.Format("Get UUT SN = {0} Result = {1} Error = {2}", sn, result, errorcode));
                    //    TestInfo.Add(keyValue + "=" + sn, errorcode);
                    //}
                    SR.Close();
                }                        
            }
            catch (Exception ex)
            {
                ShowLog.ShowErrorLog(ex.Message);
                LogHelper.Error(ex.Message);
            }
        }

       private static void ReadTestReport(string filepath, string sn,out string Testres)
        {
             Testres=string.Empty;
             try
            {
                using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {                       
                        string strLine;
                        string keyValue = string.Empty;
                        while (null != (strLine = streamReader.ReadLine()))
                        {
                                 if (!strLine.Contains("Test Data"))
                                {
                                    continue;
                                }
                                string[] txts = strLine.Split('\t');
                                if(txts.Contains(sn))
                                {
                                    Testres=txts[2];
                                }
                        }
                    }
                }
             }
            catch(Exception ex)
             {
                 ShowLog.ShowErrorLog(ex.Message);
             }
        }
        private static IntPtr SetWindowSize()
        {
            IntPtr mainHandle;
            string title = string.Empty;
            int x = 0; int y = 0;
            //bool flag = false;
            string StationName = StationInfo.StationID.ToUpper();
            if (StationName.ToUpper() == "ARFTS")
            {
                title = "ARFTS";
                x = 550; y = 150;
            }
            if (StationName.ToUpper() == "FTTS")
            {
                title = "CPE DOCSIS";
                x = 183; y = 348;
                //280 380
            }
            if (StationName.ToUpper() == "AFTSII")
            {
                title = "AFTSII-FNN";
                x = 542; y = 219;
            }
            if (title.Length < 1)
            {
                return IntPtr.Zero;
            }
            win32API window = new win32API();
            mainHandle = window.GetDesktopWindows(title);
            ShowLog.ShowTestLog("handle=" + mainHandle.ToString());
            if ((int)mainHandle < 0)
            {
                MessageBox.Show("can not find the windowhandle");
                return IntPtr.Zero;
            }
            // win32API.ShowWindow(mainHandle, 3);
            if (StationName.ToUpper() == "ARFTS")
            {
                window.MoveMouse(mainHandle, 0.595, 0.27);
            }
            else if(StationName.ToUpper()=="AFTSII")
            {
                //if (StationName.ToUpper() == "FTTS")
                //{
                //    window.MoveMouse(mainHandle, x, y);
                //    ShowLog.ShowTestLog("moveMouse=" + x.ToString() + "   " + y.ToString());                  
                //}
               //else
                //{
                    ShowLog.ShowTestLog("moveMouse=" + x.ToString() + "   " + y.ToString());
                    window.MoveMouse(mainHandle, x, y);
              //  }               
            }
            return mainHandle;
        }
        private static void EnterGrop(string Group)
        {
            if (Group == "a")
            {
                win32API.keybd_event((byte)Keys.A, 0, 0, 0);
                win32API.keybd_event((byte)Keys.A, 0, 0x2, 0);
                win32API.keybd_event((byte)Keys.A, 0, 0, 0);
                win32API.keybd_event((byte)Keys.A, 0, 0x2, 0);
                win32API.keybd_event((byte)Keys.A, 0, 0, 0);
                win32API.keybd_event((byte)Keys.A, 0, 0x2, 0);
                win32API.keybd_event((byte)Keys.Enter, 0, 0, 0);
                win32API.keybd_event((byte)Keys.Enter, 0, 0x2, 0);
            }
            if (Group == "b")
            {
                win32API.keybd_event((byte)Keys.B, 0, 0, 0);
                win32API.keybd_event((byte)Keys.B, 0, 0x2, 0);
                win32API.keybd_event((byte)Keys.B, 0, 0, 0);
                win32API.keybd_event((byte)Keys.B, 0, 0x2, 0);
                win32API.keybd_event((byte)Keys.B, 0, 0, 0);
                win32API.keybd_event((byte)Keys.B, 0, 0x2, 0);
                win32API.keybd_event((byte)Keys.Enter, 0, 0, 0);
                win32API.keybd_event((byte)Keys.Enter, 0, 0x2, 0);
            }
            sDelay.Delay(2000);
            IntPtr mainHandle;
            win32API window = new win32API();
            mainHandle = window.GetDesktopWindows("CPE DOCSIS");
            if(StationInfo.StationID=="FTTS")
            {
                //window.MoveMouse(mainHandle, 350, 235);
                win32API.keybd_event((byte)Keys.Enter, 0, 0, 0);
                win32API.keybd_event((byte)Keys.Enter, 0, 0x2, 0);
            }
        }
        private static bool JudgeIsTestStart(int TimeOutMs)
        {            
            bool flag = false;
            DateTime t = DateTime.Now.AddMilliseconds((double)TimeOutMs);
            while (!flag && DateTime.Compare(DateTime.Now, t) <= 0)
            {
                FileInfo[] logFile = new GetFileList().GetFiles(StationInfo.TelnetLogPath);
                if (StationInfo.StationID == "ARFTS" || StationInfo.StationID == "AFTSII")
                {
                    foreach (FileInfo f in logFile)
                    {                
                        if (f.Name.Contains("Telnet") || f.Name.Contains("Line"))
                        {
                            if (f.Length > 0)
                            {
                                flag = true;
                                StationInfo.ARFTS_AFTSII_test = true;
                                break;
                            }
                        }
                    }
                }
                else if (StationInfo.StationID == "FTTS")
                {
                    if(StationInfo.FTTS_test==true)
                    {
                        flag = true;
                        break;
                    }
                }
                ShowLog.ShowTestLog("Check test status");
                sDelay.Delay(500);
            }
            return flag;
        }
        static void DeleteFile(string FileFullName)
        {
            if (File.Exists(FileFullName))
            {
                File.Delete(FileFullName);
                System.Threading.Thread.Sleep(100);
                //this.log.DebugFormat("Deleted {0} file.", new object[] { FileFullName });
            }
        }

    }
}
