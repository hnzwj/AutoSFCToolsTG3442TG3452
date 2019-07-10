using SFCTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using TestTools;

namespace AutoSFCTools
{
    public partial class Form1 : Form
    {
        public Form1(LoginForm form)
        {
            InitializeComponent();
            lform = form;
            lform.Hide();
            ShowLog.richTextBox = this.rtxtBox;
        }

        int index = 1;
        TestApp Test;
        private LoginForm lform;
        MySqlDBHelper mySql = new MySqlDBHelper();
        Thread TestThread;
        dealLogs deallog = new dealLogs();
        private Sfc sfc = new Sfc();
        private static ManualResetEvent mre = new ManualResetEvent(false);
        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.Filter = "*.TXT";
            if(StationInfo.StationID=="ARFTS"||StationInfo.StationID=="AFTSII")
            {fileSystemWatcher1.Path = StationInfo.TelnetLogPath;
            fileSystemWatcher3.Path = StationInfo.TelnetLogPath;
            }
            if(StationInfo.StationID=="FTTS")
            {
                fileSystemWatcher1.Path = StationInfo.ResultLogPath;
                fileSystemWatcher3.Path = StationInfo.TelnetLogPath;
            }
            fileSystemWatcher1.IncludeSubdirectories = true;
            fileSystemWatcher1.SynchronizingObject = this;
            fileSystemWatcher1.Created += new System.IO.FileSystemEventHandler(LoadToFTP);

            fileSystemWatcher3.IncludeSubdirectories = true;
            fileSystemWatcher3.SynchronizingObject = this;
            fileSystemWatcher3.Changed += new System.IO.FileSystemEventHandler(changgeUUTColor);
        }
         private void changgeUUTColor(object sender, FileSystemEventArgs e)  
        {
           // sDelay.Delay(50000);
            btn1.BackColor = Color.Gold;
            btn1.Text = "UUT1";
            btn2.BackColor = Color.Gold;
            btn2.Text = "UUT2";
            btn3.BackColor = Color.Gold;
            btn3.Text = "UUT3";
            btn4.BackColor = Color.Gold;
            btn4.Text = "UUT4";
        }
        private void Init()
        {
            this.btn1.BackColor = Color.Gold;
            this.btn2.BackColor = Color.Gold;
            this.btn3.BackColor = Color.Gold;
            this.btn4.BackColor = Color.Gold;
            this.Text = StationInfo.StationID.ToUpper();
            if (StationInfo.Robot)
            {
                Thread threadDatabase = new Thread(new ThreadStart(WaitRobot));
                threadDatabase.IsBackground = true;
                threadDatabase.Start();
            }
            else
            {
                if (StationInfo.StationID == "ARFTS")
                {
                    getARFTScount();              
                }
                //开始监控测试Log
                fileSystemWatcher2.EnableRaisingEvents = true;
                if (StationInfo.StationID == "FTTS")
                { 
                    fileSystemWatcher2.Filter = "*.TXT";
                    fileSystemWatcher2.Path = StationInfo.ResultLogPath; 
                }
                else 
                { 
                    fileSystemWatcher2.Filter = "CPK*.TXT";
                    fileSystemWatcher2.Path = StationInfo.TelnetLogPath; 
                }
                fileSystemWatcher2.IncludeSubdirectories = true;
                fileSystemWatcher2.SynchronizingObject = this;
            }
        }

        private void StartTest(object Group)
        {
            btn1.Invoke(new Action(delegate() { btn1.BackColor = Color.Gold; btn1.Text = "testing"; }));
            btn2.Invoke(new Action(delegate() { btn2.BackColor = Color.Gold; btn2.Text = "testing"; }));
            btn3.Invoke(new Action(delegate() { btn3.BackColor = Color.Gold; btn3.Text = "testing"; }));
            btn4.Invoke(new Action(delegate() { btn4.BackColor = Color.Gold; btn4.Text = "testing"; }));
            if (TestThread == null)
            {
                TestThread = new Thread(new ParameterizedThreadStart(StartAllTest));
                TestThread.SetApartmentState(ApartmentState.MTA);
                TestThread.IsBackground = true;
                TestThread.Name = "StartTest DUT...";
                TestThread.Priority = ThreadPriority.Normal;
                TestThread.Start(Group);
            }
        }
        private void StartAllTest(object group)
        {
            PingCheck pingCheck1 = new PingCheck();
        
            /*********************************************ARFTS||AFTSII*****************************************************************/
            if (StationInfo.StationID == "ARFTS" || StationInfo.StationID == "AFTSII")
            {
                if (!pingCheck1.NetworkPing(200000, "192.168.100.1"))
                {
                    ShowLog.ShowErrorLog("ping test fail");
                    TestThread = null;
                    return;
                }
                ShowLog.ShowTestLog("ping test pass");
                //Arris_App
                Dictionary<string, string> TestInfo = new Dictionary<string, string>();
                if (0 == Arris_App.Arris_App_RunTest(group, out TestInfo))
                {
                    sfc.Open(StationInfo.IcamPort);
                    foreach (var info in TestInfo)
                    {
                        string errMsg = string.Empty;
                        string index = info.Key.Trim().Substring(info.Key.IndexOf(":") + 1, 1);
                        string sn = info.Key.Trim().Substring(info.Key.IndexOf("=") + 1);
                        string errorcode = info.Value.Trim();
                        string newTestLog = StationInfo.TelnetLogPath;
                        ShowLog.ShowTestLog("UUT" + index + ":");
                        if (sfc.GetUnitInfo(sn))
                        {
                            if (errorcode == "PASS" || (errorcode != "" && errorcode != "UNKOWN"))
                            {

                                sfc.SaveResult(sn, errorcode, out errMsg);
                                if (errMsg.Length > 0)
                                {
                                    ShowLog.ShowTestLog("UUT" + index + ":" + errMsg);
                                }
                            }
                        }
                        if ("1" == index)
                        {
                            btn1.Invoke(new Action(delegate() { btn1.BackColor = Color.Lime; btn1.Text = sn; }));
                        }
                        if ("2" == index)
                        {
                            btn2.Invoke(new Action(delegate()
                            {
                                btn2.BackColor = Color.Lime;
                                btn2.Text = sn;
                            }));
                        }
                        if ("3" == index)
                        {
                            btn3.Invoke(new Action(delegate()
                            {
                                btn3.BackColor = Color.Lime;
                                btn3.Text = sn;
                            }));
                        }
                        if ("4" == index)
                        {
                            btn4.Invoke(new Action(delegate()
                            {
                                btn4.BackColor = Color.Lime;
                                btn4.Text = sn;
                            }));
                        }
                        if (errorcode == "PASS")
                        {
                            string sqlUpdate = string.Format("UPDATE `tg2482_auto`.`{0}_test` SET `status`='Passing' WHERE `sn1`='{1}{2}{3}'  AND `fixture`= ({4})", StationInfo.StationID, group, group, group, index);
                            mySql.updataResult(sqlUpdate);
                            string res = "pass";
                            NewLogParameter Newlogdata = new NewLogParameter(sn, newTestLog, res);
                            //object data = Newlogdata.Path;
                            deallog.MoveTestLog(Newlogdata);
                        }
                        else if (errorcode != "" && errorcode != "UNKOWN")
                        {
                            string sqlUpdate = string.Format("UPDATE `tg2482_auto`.`{0}_test` SET `status`='Failing' WHERE `sn1`='{1}{2}{3}'  AND `fixture` = ({4})", StationInfo.StationID, group, group, group, index);
                            mySql.updataResult(sqlUpdate);
                            string res = "fail";
                            NewLogParameter Newlogdata = new NewLogParameter(sn, newTestLog, res);
                            //object data = Newlogdata.Path;
                            deallog.MoveTestLog(Newlogdata);
                        }
                    }
                    sfc.Close();
                }
                sDelay.Delay(1000);
                TestThread = null;
                object obj = StationInfo.TelnetLogPath;// testLogPath;
                Thread backupLog = new Thread(new ParameterizedThreadStart(deallog.MoveOldLog));
                backupLog.IsBackground = true;
                backupLog.Start(obj);
            }

            /************************************FTTS***************************************************/
            if (StationInfo.StationID == "FTTS")
            {
                Test = new TestApp(rtxtBox, "123456");
                sDelay.Delay(500);
                if (!Test.TelnetToLogin("172.20.1.251"))
                {
                    ShowLog.ShowErrorLog("ping test fail");
                    TestThread = null;
                    return;
                }
                ShowLog.ShowTestLog("ping test pass");
                string[] testinfo={"0","1","2","3","4","5","6"};
                //Arris_App
                Dictionary<string, string> TestInfo = new Dictionary<string, string>();
                if (0 == Arris_App.Arris_App_RunTest(group, out TestInfo))
                {
                    sfc.Open(StationInfo.IcamPort);
                    foreach (var info in TestInfo)
                    {
                        for (int i = 0; i < 5;i++ )
                        {
                            testinfo[i] = info.Key.Trim();
                        }
                           
                    }
                    foreach (var info in TestInfo)
                    {
                        string errMsg = string.Empty;
                        string sn = info.Key.Trim().Substring(info.Key.IndexOf("=") + 1);
                        string errorcode = info.Value.Trim();
                        string newTestLog = StationInfo.TelnetLogPath;
                        if(index>0)
                        {
                            ShowLog.ShowTestLog("UUT" + index + ":");
                            if (sfc.GetUnitInfo(sn))
                            {
                                if (errorcode == "PASS" || (errorcode != "" && errorcode != "UNKOWN"))
                                {

                                    sfc.SaveResult(sn, errorcode, out errMsg);
                                    if (errMsg.Length > 0)
                                    {
                                        ShowLog.ShowTestLog("UUT" + index + ":" + errMsg);
                                    }
                                }
                            }
                            if (1 == index)
                            {
                                btn1.Invoke(new Action(delegate() { btn1.BackColor = Color.Lime; btn1.Text = sn; }));
                            }
                            if (2 == index)
                            {
                                btn2.Invoke(new Action(delegate()
                                {
                                    btn2.BackColor = Color.Lime;
                                    btn2.Text = sn;
                                }));
                            }
                            if (3 == index)
                            {
                                btn3.Invoke(new Action(delegate()
                                {
                                    btn3.BackColor = Color.Lime;
                                    btn3.Text = sn;
                                }));
                            }
                            if (4 == index)
                            {
                                btn4.Invoke(new Action(delegate()
                                {
                                    btn4.BackColor = Color.Lime;
                                    btn4.Text = sn;
                                }));
                            }
                            if (errorcode == "PASS")
                            {
                                string sqlUpdate = string.Format("UPDATE `tg2482_auto`.`{0}_test` SET `status`='Passing' WHERE `sn1`='{1}{2}{3}'  AND `fixture`= ({4})", StationInfo.StationID, group, group, group, index);
                                mySql.updataResult(sqlUpdate);
                                //ShowLog.ShowTestLog("now index=" + index);
                            }
                            else if (errorcode != "" && errorcode != "UNKOWN")
                            {
                                string sqlUpdate = string.Format("UPDATE `tg2482_auto`.`{0}_test` SET `status`='Failing' WHERE `sn1`='{1}{2}{3}'  AND `fixture` = ({4})", StationInfo.StationID, group, group, group, index);
                                mySql.updataResult(sqlUpdate);
                            }
                        }
                        if (index == StationInfo.TotalUUT)
                        {
                            string res = "";
                            NewLogParameter Newlogdata = new NewLogParameter(sn, newTestLog, res);
                            deallog.MoveTestLog(Newlogdata);
                        }
                        index--;
                        sfc.Close();
                    }
                }
                sDelay.Delay(1000);
                TestThread = null;
                object obj = StationInfo.ResultLogPath;// testLogPath;
                Thread backupLog = new Thread(new ParameterizedThreadStart(deallog.MoveOldLog));
                backupLog.IsBackground = true;
                backupLog.Start(obj);
            }
        }

        private void WaitRobot()
        {
            int i = 0;
            while (true)
            {
                string group = "C";
                string sqla = string.Format("select count(sn1) from {0}_test where `status`='Test' and sn1='aaa' and fixture in ({1})", StationInfo.StationID, StationInfo.Fixture);
                string sqlb = string.Format("select count(sn1) from {0}_test where `status`='Test'and sn1='bbb' and fixture in ({1})", StationInfo.StationID, StationInfo.Fixture);
                object obj = null;
                obj = mySql.selectResult(sqla);
                if (4 == int.Parse(obj.ToString()))
                {
                    group = "a";
                }
                else
                {
                    obj = mySql.selectResult(sqlb);
                    if (4 == int.Parse(obj.ToString()))
                    {
                        group = "b";
                    }
                }

                string sqlUpdate = string.Format("UPDATE `tg2482_auto`.`{0}_test` SET `status`='Testing' WHERE `sn1`='{1}{2}{3}'  AND `fixture` in ({4})", StationInfo.StationID, group, group, group, StationInfo.Fixture);
                if ((group == "a" || group == "b") && TestThread == null)
                {
                    string testlog = string.Format("Found {0} waiting test", group);
                    ShowLog.ShowTestLog(testlog);
                    if (mySql.updataResult(sqlUpdate) > 0)
                    {
                        testlog = string.Format("Start {0} testing", group);
                        ShowLog.ShowTestLog(testlog);
                        StartTest(group);
                    }
                }
                else if (TestThread == null)
                {
                    if (i == 0)
                    {
                        string testlog = string.Format("waiting for DUT......");
                        ShowLog.ShowTestLog(testlog);
                        i++;
                    }
                }
                sDelay.Delay(1000);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            TestThread = null;
            Application.Exit();
        }

        private void LoadToFTP(object sender, FileSystemEventArgs e)   
        //private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            getARFTScount();

            int flag = 0;
            string dateTime = DateTime.Now.ToShortDateString();
            string Yesterday = DateTime.Now.AddDays(-1).ToShortDateString();

            DateTime dt = DateTime.Now;
            DateTime yes = DateTime.Now.AddDays(-1);
            string today = dt.ToString("dddd MMMM dd yyyy", DateTimeFormatInfo.InvariantInfo);
            string yesterday = yes.ToString("dddd MMMM dd yyyy", DateTimeFormatInfo.InvariantInfo);

            string FA = string.Empty;
            if (StationInfo.StationID == "ARFTS" || StationInfo.StationID == "AFTSII")
            {
                FA = StationInfo.TelnetLogPath.ToString();
            }
            if(StationInfo.StationID=="FTTS")
            {
                FA=StationInfo.ResultLogPath.ToString();
            }
            FileInfo[] fl=new GetFileList().GetFiles(FA);
            foreach (FileInfo f in fl)
            {
                if(StationInfo.StationID=="ARFTS"||StationInfo.StationID=="AFTSII")
                {
                     if (f.FullName.Contains(dateTime)&&f.FullName.Contains("CPK"))
                     {
                         flag = 1;
                     }
                }
                if(StationInfo.StationID=="FTTS")
                {
                    if (f.FullName.Contains(today))
                    {
                        flag = 1;
                    }
                }               
            }
            if (flag == 1)
            {
                foreach (FileInfo f in fl)
                {
                    if (f.FullName.Contains(Yesterday)&&f.FullName.Contains("CPK")||f.FullName.Contains(yesterday))
                    {
                        string FTP = @"ftp://200.168.16.2/";
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

                        if (!Directory.Exists(stationAdress + StationInfo.stationNumber))
                        {
                            ftpHpler.MakeDir(StationInfo.stationNumber.ToString());
                        }

                        FtpRemotePath = FtpRemotePath + "/" + StationInfo.stationNumber.ToString();
                        ftpHpler = new FTPHelper("200.168.16.2", FtpRemotePath, "te", "te123.");
                        sDir = ftpHpler.GetFilesDetailList();
                     
                        if (!Directory.Exists(stationAdress + DateTime.Now.ToString("yyyy-MM")))
                        {
                            ftpHpler.MakeDir(DateTime.Now.ToString("yyyy-MM"));
                        }
                        FtpRemotePath = FtpRemotePath + "/" + DateTime.Now.ToString("yyyy-MM");  // DateTime.Now.ToString("dd");
                       // ftpHpler = new FTPHelper("200.168.16.2", FtpRemotePath, "te", "te123.");
                        //sDir = ftpHpler.GetFilesDetailList();
                        //if (!Directory.Exists(FtpRemotePath + DateTime.Now.ToString("dd")))
                        //{
                        //    ftpHpler.MakeDir(DateTime.Now.ToString("dd"));
                        //}
                        //FtpRemotePath = FtpRemotePath + "/" + DateTime.Now.ToString("dd");
                        ftpHpler = new FTPHelper("200.168.16.2", FtpRemotePath, "te", "te123.");
                        ftpHpler.Upload(f.FullName);
                        //newName = string.Empty;
                    }                 
                }
            }
        }

        private void fileSystemWatcher2_Changed(object sender, FileSystemEventArgs e)
        {
            if (TestThread == null)
            {
                TestThread = new Thread(NormalTest);
                TestThread.SetApartmentState(ApartmentState.MTA);
                TestThread.IsBackground = true;
                TestThread.Name = "StartTest DUT...";
                TestThread.Priority = ThreadPriority.Normal;
                TestThread.Start();
            }
        }

        private void NormalTest()
        {
                sDelay.Delay(1000);
                FileInfo[] fl;
                bool res=false;
                Dictionary<string, string> TestInfo = new Dictionary<string, string>();
                Dictionary<string, string> dataInfo = new Dictionary<string, string>();
              
                string[] testinfo = { "0", "1", "2", "3"};
                string[] value = { "0", "1", "2", "3" };
                if (StationInfo.StationID == "ARFTS" || StationInfo.StationID == "AFTSII")
                {  
                    string testTime = DateTime.Now.ToShortDateString().ToString();
                    fl = new GetFileList().GetFiles(StationInfo.TelnetLogPath.ToString());
                    if (fl.Length > 0)
                    {           
                        foreach (FileInfo f in fl)
                        {
                           if (f.Name.Contains("CPK_") &&f.Name.Contains(testTime))//测试完毕
                            {                                
                                Arris_App.AnaylzerLog(f.FullName, out TestInfo);
                                break;
                            }
                        }
                    }
                }
                if (StationInfo.StationID == "FTTS")
                {
                    DateTime dt = DateTime.Now;
                    string day = dt.ToString("dddd  MMMM dd  yyyy", DateTimeFormatInfo.InvariantInfo);
                    //Friday  May 24  2019 LineF8TG34XX 3.txt
                    fl = new GetFileList().GetFiles(StationInfo.ResultLogPath.ToString());
                    if (fl.Length > 0)
                    {
                        foreach (FileInfo f in fl)
                        {
                            if (f.Name.Contains("Line") && f.Name.Contains(day))
                            {
                                //LineF8 Thursday  June 18  2019.txt
                                //       Tuesday  June 18  2019
                                Arris_App.AnaylzerLog(f.FullName, out TestInfo);
                                break;
                            }
                        }
                    }
                }
            if(TestInfo.Count!=0)
            {
                int a = StationInfo.TotalUUT;
                foreach (var info in TestInfo)
                {
                    if (a > 0)
                    {
                        testinfo[a - 1] = info.Key.Trim();
                        value[a - 1] = info.Value.Trim();
                        a--;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    dataInfo.Add(testinfo[i], value[i]);
                }
            }
            else
            {
                ShowLog.ShowTestLog("can't find the new log");
            }
                  
                sfc.Open(StationInfo.IcamPort);
                foreach(var data in dataInfo)
                {
                    string errMsg = string.Empty;
                    string index0=data.Key.Trim().Substring(data.Key.IndexOf(":") + 1, 1);
                    string sn = data.Key.Trim().Substring(data.Key.IndexOf("=") + 1);
                    string errorcode = data.Value.Trim();
                    if(errorcode=="P")
                    {
                        errorcode = "PASS";
                    }
                    string msg = string.Format(" TestInfo = {0} Error = {1}", sn, errorcode);
                    LogHelper.Info(msg);

                    if (StationInfo.StationID == "ARFTS" || StationInfo.StationID == "AFTSII")
                    {
                        index = 0;
                        ShowLog.ShowTestLog("UUT" + index0 + ":" + errMsg);
                        if (sfc.GetUnitInfo(sn))
                        {
                            if (errorcode == "PASS" || (errorcode != "" && errorcode != "UNKOWN"))
                            {
                                sfc.SaveResult(sn, errorcode, out errMsg);
                            }
                        }
                    }
                    if (StationInfo.StationID == "FTTS")
                    {
                        index0 = "0";
                        if (index <StationInfo.TotalUUT||index==StationInfo.TotalUUT)
                        {
                            ShowLog.ShowTestLog("UUT" + index + ":" + errMsg);
                            if (sfc.GetUnitInfo(sn))
                            {
                                if (errorcode == "PASS" || (errorcode != "" && errorcode != "UNKOWN"))
                                {
                                    sfc.SaveResult(sn, errorcode, out errMsg);
                                }
                            }
                        }
                      
                    }
                    string MSg="";
                    if (errorcode == "PASS")
                    {
                        res = true;
                    }
                    else
                    {
                        res = false;
                        MSg = errorcode;
                    }
                    if(StationInfo.StationID=="ARFTS"||StationInfo.StationID=="AFTSII")
                    {
                        DisplaySerialNumberOnUI(sn,index0, res, MSg);
                    }
                    else
                    {
                        DisplaySerialNumberOnUI(sn,index.ToString(), res, MSg);
                        index++; 
                    }
                    sfc.Close();
                }
                res = false;
                index =1;
                TestThread = null;            
        }

        private void DisplaySerialNumberOnUI(string sn,string Socket, bool Pass, string MSg)
        {
            Button btn = null;
            if ("1" == Socket)
            {
                btn = btn1;
            }
            if ("2" == Socket)
            {
                btn = btn2;
            }
            if ("3" == Socket)
            {
                btn = btn3;
            }
            if ("4" == Socket)
            {
                btn = btn4;
            }
            if (btn != null)
            {
                if (Pass && StationInfo.FLAG == 1)
                {
                    btn.Invoke(new Action(delegate()
                    {
                        btn.Text = sn;
                        btn.BackColor = Color.Lime;
                    }));
                }
                else
                {
                    btn.Invoke(new Action(delegate()
                    {                  
                        btn.BackColor = Color.Red;
                        if (StationInfo.FLAG != 1)
                        { 
                           btn.Text= "the SN is not valid"; 
                        }
                        else
                        {
                            btn.Text = MSg;
                        }
                    }));
                }
            }
        }
        private static void getARFTScount()
        {
            int LogLength = 0;
            string dataTime = DateTime.Now.ToShortDateString().ToString();
            ArrayList arraylist = new ArrayList();
            string strLine = string.Empty;
            StationInfo.LogCount = 0;
            FileInfo[] fl = new GetFileList().GetFiles(StationInfo.TelnetLogPath.ToString());
            if (fl.Length > 0)
            {
                foreach (FileInfo f in fl)
                {
                    if (f.FullName.Contains("CPK_") && f.Name.Contains(dataTime))
                    {
                        StreamReader streamReader = new StreamReader(f.FullName);
                        while ((strLine = streamReader.ReadLine()) != null)
                        {
                            arraylist.Add(strLine);
                        }
                        LogLength = arraylist.Count;
                        streamReader.Close();
                        StationInfo.LogCount = LogLength;
                        break;
                    }
                }
            }

        }
    }
}



