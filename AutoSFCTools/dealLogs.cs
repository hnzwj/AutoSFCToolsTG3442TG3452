using AutoSFCTools;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections;

namespace AutoSFCTools
{
    public class dealLogs
    {
        public AutoResetEvent[] autoEvents = null;
        public AutoResetEvent MoveOld = null;
        public AutoResetEvent MoveNewTestlog = null;
        public AutoResetEvent MoveResultLog = null;
        string AllSN = string.Empty;
        int sn = 0;
        string[] currentsn = { "1", "2", "3", "4", "5" };
      
        public dealLogs()
        {
            //在内存中保持着一个bool值，如果bool值为False，则使线程阻塞
           // autoEvents = new AutoResetEvent[]
           //{
           //    new AutoResetEvent(false),
           //    new AutoResetEvent(false),
           //    new AutoResetEvent(false)
           //};

            MoveOld = new AutoResetEvent(false);
            MoveNewTestlog =  new AutoResetEvent(false) ;
            MoveResultLog = new AutoResetEvent(false);

        }
        /// <summary>
        /// 清除历史测试记录
        /// </summary>
        public void MoveOldLog(object FoldePath)
        {
            string OldTestLogPath = @"C:\StationLogs";
            if (!Directory.Exists(OldTestLogPath))
            {
                Directory.CreateDirectory(OldTestLogPath);
            }
            OldTestLogPath = OldTestLogPath + "\\Old";
            if (!Directory.Exists(OldTestLogPath))
            {
                Directory.CreateDirectory(OldTestLogPath);
            }
            if (!Directory.Exists(FoldePath.ToString()))
            {
                MoveOld.Set();
                return;
            }
            FileInfo[] fl = new GetFileList().GetFiles(FoldePath.ToString());
            foreach (FileInfo f in fl)
            {
                File.Move(f.FullName, OldTestLogPath + "\\" + f.Name + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");
            }
            //autoEvents[0].Set(); //Set方法发送信号到等待线程以继续其工作
            MoveOld.Set();
        }
        public  void MoveResult(object FoldePath)
        {
            string newName = "123";
            string ResultLogPath = @"C:\StationLogs" + "\\Reult";
            string SNS = string.Empty;
            if (!Directory.Exists(ResultLogPath))
            {
                Directory.CreateDirectory(ResultLogPath);
            }
            ResultLogPath = ResultLogPath + "\\" + DateTime.Now.ToString("yyyy-MM");
            if (!Directory.Exists(ResultLogPath))
            {
                Directory.CreateDirectory(ResultLogPath);
            }
            ResultLogPath = ResultLogPath + "\\" + DateTime.Now.ToString("dd");
            if (!Directory.Exists(ResultLogPath))
            {
                Directory.CreateDirectory(ResultLogPath);
            }
            if (!Directory.Exists(FoldePath.ToString()))
            {
                MoveResultLog.Set();
                return;
            }
            FileInfo[] fl = new GetFileList().GetFiles(FoldePath.ToString());
            foreach (FileInfo f in fl)
            {
                if (f.Name.Contains("CPK")||f.Name.Contains("Line"))
                {
                    using (FileStream fileStream = new FileStream(f.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            string strLine;
                           
                            while (null != (strLine = streamReader.ReadLine()))
                            {
                                if (StationInfo.StationID == "ARFTS")
                                {
                                    if (!strLine.Contains("UUT:"))
                                    {
                                        continue;
                                    }
                                    string[] txts = strLine.Split('\t');
                                    if (txts.Length == 6)
                                    {
                                        string sn = txts[1];
                                        if (sn.Length < 1)
                                        {
                                            continue;
                                        }
                                        SNS+=sn+ "_";
                                    }
                                }
                                if (StationInfo.StationID == "AFTSII")
                                {
                                    if (!strLine.Contains("UUT:"))
                                    {
                                        continue;
                                    }
                                    string[] txts = strLine.Split('\t');
                                    if (txts.Length == 9)
                                    {
                                        string sn = txts[3];
                                        {
                                            if (sn.Length < 1)
                                            {
                                                continue;
                                            }
                                        }
                                        SNS += sn + "_";
                                    }
                                }
                                if(StationInfo.StationID=="FTTS")
                                {
                                    string[] txts = strLine.Split('\t');
                                    string sn = txts[3];
                                        {
                                            if (sn.Length < 1)
                                            {
                                                continue;
                                            }
                                        }
                                        SNS += sn + "_";                                    
                                }
                            }
                            fileStream.Close();
                            streamReader.Close();
                        }
                    }
                    newName = string.Format("{0}_{1}_{2}.txt", StationInfo.StationID, SNS, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                    File.Move(f.FullName, ResultLogPath + "\\" + newName);
                    MoveResultLog.Set();
                    // ShowLog.ShowTestLog("complete a test,and move testLo g succeed!");
                    break;
                }
            }
        }


        public void MoveTestLog(object data)
        {
           // if (data is NewLogParameter)
           // {
                NewLogParameter ap = (NewLogParameter)data;
                string FoldePath = ap.Path;
                string SN = ap.SN;
                string Res = ap.Result;
                string Model = "";
                string NewTestLogPath = @"C:\StationLogs";
                string CurrentSn = "123456";
                bool flag = false;
                string newName="123";
                if (!Directory.Exists(NewTestLogPath))
                {
                    Directory.CreateDirectory(NewTestLogPath);
                }
                NewTestLogPath = NewTestLogPath + "\\" + DateTime.Now.ToString("yyyy-MM");
                if (!Directory.Exists(NewTestLogPath))
                {
                    Directory.CreateDirectory(NewTestLogPath);
                }
                NewTestLogPath = NewTestLogPath + "\\" + DateTime.Now.ToString("dd");
                if (!Directory.Exists(NewTestLogPath))
                {
                    Directory.CreateDirectory(NewTestLogPath);
                }
                FileInfo[] fl = new GetFileList().GetFiles(StationInfo.TelnetLogPath);
                if(data is NewLogParameter)
                {
                    if (StationInfo.StationID == "ARFTS" || StationInfo.StationID == "AFTSII")
                    {
                        foreach (FileInfo f in fl)
                        {
                            if (f.Name.Contains("CPK"))
                            {
                                continue;
                            }
                            if (!File.Exists(f.FullName))
                            {
                                continue;
                            }
                            using (FileStream fileStream = new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                StreamReader streamReader = new StreamReader(fileStream);
                                string strLine;
                                while (null != (strLine = streamReader.ReadLine()))
                                {
                                    if (strLine.Contains("Cable Modem Serial Number"))
                                    {
                                        string[] array = strLine.Split(new string[] { "-", "<", ">" }, StringSplitOptions.None);
                                        if (array.Length > 1)
                                        {
                                            CurrentSn = array[2].Trim();
                                        }
                                    }
                                    if (strLine.Contains("Hardware Model:"))
                                    {
                                        string[] array1 = strLine.Split(new string[] { ":" }, StringSplitOptions.None);
                                        if (array1.Length > 1)
                                        {
                                            Model = array1[1].Trim();
                                        }
                                    }
                                    //if (SN == CurrentSn && Model != null)
                                    if (CurrentSn.Length > 13 && Model.Length > 3)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                fileStream.Close();
                                streamReader.Close();
                            }
                            if (flag && SN == CurrentSn)
                            {
                                newName = string.Format("{0}_{1}_{4}_{2}_{3}.txt", StationInfo.StationID, SN, Model, Res, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                                File.Move(f.FullName, NewTestLogPath + "\\" + newName);
                                MoveNewTestlog.Set();
                               // ShowLog.ShowTestLog("complete a test,and move testLog succeed!");
                                break;
                            }
                        }
                    }
                }
                
            //FTTS
                if (StationInfo.StationID == "FTTS")
                {
                    //ShowLog.ShowTestLog("123");
                    foreach (FileInfo f in fl)
                    {                     
                        using (FileStream fileStream = new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            StreamReader streamReader = new StreamReader(fileStream);
                            string strLine;
                            while (null != (strLine = streamReader.ReadLine()))
                            {                                
                                if (strLine.Contains("Cable Modem Serial Number"))
                                {
                                    string[] array = strLine.Split(new string[] { "-", "<", ">" }, StringSplitOptions.None);
                                    if (array.Length > 1)
                                    {
                                        currentsn[sn] = array[2].Trim();
                                        if(sn==0||currentsn[sn]!=currentsn[sn-1])
                                        {
                                            AllSN += currentsn[sn] + "_";
                                            sn++;
                                            //ShowLog.ShowTestLog(AllSN);
                                            //ShowLog.ShowTestLog(sn.ToString());
                                        }                                       
                                    }
                                }
                                if(sn==StationInfo.TotalUUT)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            fileStream.Close();
                            streamReader.Close();
                        }
                        if (sn == StationInfo.TotalUUT)
                        {
                            newName = string.Format("{0}_{1}_{2}.txt", StationInfo.StationID, AllSN, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                            File.Move(f.FullName, NewTestLogPath + "\\" + newName);
                            MoveNewTestlog.Set();
                            //ShowLog.ShowTestLog("complete a test,and move testLog succeed!"); 
                            AllSN = string.Empty;
                            break;
                        }                       
                    }                                           
                }
               //Copy to FTP
                string FTP=@"ftp://200.168.16.2/";
                string[] sDir = null;
                string FtpRemotePath = StationInfo.StationID + "/";
                FTPHelper ftpHpler = new FTPHelper("200.168.16.2", ".", "te", "te123.");
                sDir = ftpHpler.GetFilesDetailList();
                string stationAdress = FTP + FtpRemotePath;
                if (!Directory.Exists(stationAdress))
                {
                    ftpHpler.MakeDir(StationInfo.StationID);
                }              
                ftpHpler = new FTPHelper("200.168.16.2", FtpRemotePath, "te", "te123.");
                sDir = ftpHpler.GetFilesDetailList();
                if (!Directory.Exists(stationAdress + DateTime.Now.ToString("yyyy-MM")))
                {
                    ftpHpler.MakeDir(DateTime.Now.ToString("yyyy-MM"));
                }              
                FtpRemotePath = FtpRemotePath + "/" + DateTime.Now.ToString("yyyy-MM");  // DateTime.Now.ToString("dd");
                ftpHpler = new FTPHelper("200.168.16.2", FtpRemotePath, "te", "te123.");
                sDir = ftpHpler.GetFilesDetailList();
                if (!Directory.Exists(FtpRemotePath+DateTime.Now.ToString("dd")))
                {
                    ftpHpler.MakeDir(DateTime.Now.ToString("dd"));
                }               
                FtpRemotePath = FtpRemotePath + "/" + DateTime.Now.ToString("dd");
                ftpHpler = new FTPHelper("200.168.16.2", FtpRemotePath, "te", "te123.");
                ftpHpler.Upload(NewTestLogPath + "\\" + newName);
                newName = string.Empty;
        }       
    }
}
