using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Reflection;
using TestTools;

namespace AutoSFCTools
{
    class TestApp
    {

        IPworks_Telnet _IPworks_Telnet;
        

        RichTextBox richTextBox;
        ComboBox combobox;
        public string dateRe;

        private string sARM_IP = "192.168.100.1";
        private string sATOMIP = "192.168.100.3";

        private string sUUT_Client_Arm_IP;
        private string sUUTSN = "";

        public static int sFWCheck = 0;
        public static int sCSFWCheck = 0;


        public TestApp(RichTextBox textBox, string sn)
        {
            this.richTextBox = textBox;
            //this.combobox = combobox;
            this.sUUTSN = sn;
            this._IPworks_Telnet = new IPworks_Telnet();
          
          
        }
        public TestApp(RichTextBox textBox, string sn, string sPortName)
        {
            this.richTextBox = textBox;
            //this.combobox = combobox;
            this.sUUTSN = sn;
            this._IPworks_Telnet = new IPworks_Telnet();
           

            AddLog("Using:" + sPortName);
        }          
        public bool TelnetToLogin(string reMoteIP)
        {
            try
            {
                if (!_IPworks_Telnet._telnet.Connected)
                {
                    _IPworks_Telnet._telnet.Timeout = 10;
                    //_IPworks_Telnet._telnet.LocalHost = sLocalIP;
                    //telnet1.LocalPort = 0;
                    _IPworks_Telnet._telnet.RemoteHost = reMoteIP;
                    _IPworks_Telnet._telnet.InvokeThrough = this.richTextBox;
                    _IPworks_Telnet._telnet.Connected = true;
                    if (_IPworks_Telnet._telnet.Connected)
                    {
                        if (string.IsNullOrEmpty(_IPworks_Telnet.dateRe))
                        {
                            sDelay.Delay(1000);
                            //ShowLog(_IPworks_Telnet.dateRe);
                            if (_IPworks_Telnet.dateRe.Contains("Login"))
                            {
                                _IPworks_Telnet.Write("foxconn");
                                sDelay.Delay(1000);
                               // ShowLog(_IPworks_Telnet.dateRe);                               
                            }
                            if (_IPworks_Telnet.dateRe.Contains("Password"))
                            {
                                 _IPworks_Telnet.Write("foxconn");
                                 sDelay.Delay(1000);
                                  //ShowLog(_IPworks_Telnet.dateRe);
                            }
                            if (_IPworks_Telnet.dateRe.Contains("E6000"))
                            {
                                  //return true;
                                 _IPworks_Telnet.Write("show cable modem format factory cable-mac 5");
                                  sDelay.Delay(1000);
                                 // ShowLog(_IPworks_Telnet.dateRe);
                             }
                            if (_IPworks_Telnet.dateRe.Contains("172.20.6"))
                                {
                                    string text=_IPworks_Telnet.dateRe;
                                    string[] ip = new string[1000];
                                    int i = 0;
                                    System.IO.StringReader sr = new System.IO.StringReader(text);
                                    while (sr.Peek() >= 0)
                                    {
                                        string line = sr.ReadLine();
                                        if (line.Contains("172.20.6"))
                                        {                                           
                                            i++;
                                        }
                                    }
                                if(i==StationInfo.TotalUUT)
                                {
                                    return true;
                                }                                                            
                                }
                            else
                            {
                                int a = _IPworks_Telnet.Write("");
                            }
                        }

                    }
                    else
                    {
                        _IPworks_Telnet._telnet.Connected = false;
                        return false;
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                _IPworks_Telnet._telnet.Connected = false;
                return false;
            }
        }

        private string channelId = "0";
  

        public bool secureLockCheck(string[] lookfor)
        {
            int iPassCount = 0;
            for (int i = 0; i < lookfor.Length; i++)
            {
                if (dateRe.IndexOf(lookfor[i]) >= 0)
                {
                    iPassCount++;
                    ShowLog("Look for:" + lookfor[i] + " is PASS");

                }


            }
            if (iPassCount >= 4)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool ExeCommand(string command, string strToLookFor, string TimeOut, string linkMethod)
        {

            try
            {

                int timeout = Convert.ToInt32(TimeOut);
                AddLog("SendCommand:" + command);
                dateRe = "";
                if (linkMethod == "telnet")
                {
                    int a = _IPworks_Telnet.SendReceive(command, strToLookFor, timeout, out dateRe);
                    ShowLog(dateRe);
                    if (a != 0)
                    {
                        return false;
                    }

                }
               
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                return false;
            }
            return true;
        }
       
        
     
        public bool formatDumpData(string[] input)
        {

            string ssn = "";
            string sunitFW = "";
            string sunitCSFW = "";
            string sBand = "";
            string sFactoryMode = "";
            string sTelnetEnable = "";
            string sISN = "";  //ZAS99A183100D7C
            string seSAFEs = "";
            string sWPSPin = "";
            string sPSKKey = "";

            char[] charSeparators = new char[] { '\n' };
            string[] temp = dateRe.Trim().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int x = 0; x < temp.Length; x++)
            {
                if (temp[x].Contains("Cable Modem Serial Number"))
                {

                    int indexStart = temp[x].IndexOf("<");
                    int indexEnd = temp[x].IndexOf(">");
                    ssn = temp[x].Substring(indexStart + 1, indexEnd - indexStart - 1).Trim(); ;


                }
                if (temp[x].Contains("Cable Modem Sector 1 file name"))
                {

                    int indexStart = temp[x].IndexOf("<");
                    int indexEnd = temp[x].IndexOf(">");
                    sunitFW = temp[x].Substring(indexStart + 1, indexEnd - indexStart - 1).Trim(); ;


                }
                if (temp[x].Contains("Cable Modem Sector 2 file name"))
                {


                    int indexStart = temp[x].IndexOf("<");
                    int indexEnd = temp[x].IndexOf(">");
                    sunitCSFW = temp[x].Substring(indexStart + 1, indexEnd - indexStart - 1).Trim(); ;

                }
                if (temp[x].Contains("DS&US frequency range active band"))
                {



                    int indexStart = temp[x].IndexOf("<");
                    int indexEnd = temp[x].IndexOf(">");

                    sBand = temp[x].Substring(indexStart + 1, indexEnd - indexStart - 1).Trim(); ;



                }
                if (temp[x].Contains("Factory Mode"))
                {

                    int indexStart = temp[x].IndexOf("-");


                    sFactoryMode = temp[x].Substring(indexStart + 1, temp[x].Length - indexStart - 1).Trim();



                }
                if (temp[x].Contains("Telnet Enable"))
                {


                    int indexStart = temp[x].IndexOf("-");


                    sTelnetEnable = temp[x].Substring(indexStart + 1, temp[x].Length - indexStart - 1).Trim();


                }
                if (temp[x].Contains("ISN"))
                {
                    int indexStart = temp[x].IndexOf("-");

                    sISN = temp[x].Substring(indexStart + 1, temp[x].Length - indexStart - 1).Trim();

                }
                if (temp[x].Contains("Init eSAFEs"))
                {

                    int indexStart = temp[x].IndexOf("-");

                    seSAFEs = temp[x].Substring(indexStart + 1, temp[x].Length - indexStart - 1).Trim();


                }
                if (temp[x].Contains("WPS PIN"))
                {

                    int indexStart = temp[x].IndexOf("-");

                    sWPSPin = temp[x].Substring(indexStart + 1, temp[x].Length - indexStart - 1).Trim();

                }
                if (temp[x].Contains("PSK Key"))
                {

                    int indexStart = temp[x].IndexOf("-");
                    sPSKKey = temp[x].Substring(indexStart + 1, temp[x].Length - indexStart - 1).Trim();

                }

            }

            string sLog = "";
            if (sFactoryMode != input[0])
            {
                sLog = string.Format("Factory Mode:{0},is not match with {1} ", sFactoryMode, input[0]);
                AddLog(sLog);
                return false;
            }
            sLog = string.Format("Factory Mode:{0},is match with {1}", sFactoryMode, input[0]);
            AddLog(sLog);

            if (sTelnetEnable != input[1])
            {
                sLog = string.Format("Telnet Enable:{0},is not match with {1} ", sTelnetEnable, input[1]);
                AddLog(sLog);
                return false;
            }
            sLog = string.Format("Telnet Enable:{0},is match with {1} ", sTelnetEnable, input[1]);
            AddLog(sLog);

            if (seSAFEs != input[2])
            {
                sLog=string.Format("Init eSAFEs:{0},is not match with {1} " +seSAFEs, input[2]);
                AddLog(sLog);
                return false;
            }

            sLog = string.Format("Init eSAFEs:{0},is  match with {1} " + seSAFEs, input[2]);
            AddLog(sLog);

            return true;


        }
 


        delegate void SetTextCallback(string text);
        private void ShowLog(string text)
        {
            if (this.richTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ShowLog);
                this.richTextBox.Invoke(d, new object[] { text });
            }
            else
            {
                richTextBox.AppendText(text + System.Environment.NewLine);
                // richTextBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "\t" + text + System.Environment.NewLine);
            }
        }
        private void AddLog(string text)
        {

            if (this.richTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddLog);
                this.richTextBox.Invoke(d, new object[] { text });
            }
            else
            {
                //richTextBox.AppendText(text + System.Environment.NewLine);
                richTextBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "\t" + text + System.Environment.NewLine);
            }
        }
        public void exitProcess()
        {

            if (this._IPworks_Telnet._telnet.Connected)
            {
                string sCommand = "/ !logout";
                this._IPworks_Telnet.Write(sCommand);
                this._IPworks_Telnet._telnet.Disconnect();
              
                AddLog(sCommand);
            }
         
        }
        public bool stopScanSerial()
        {
            return true;

        }
    


    }
}


