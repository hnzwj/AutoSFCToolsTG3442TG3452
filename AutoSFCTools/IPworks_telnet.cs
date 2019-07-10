using System;
using System.Collections.Generic;
using System.Text;

using nsoftware.IPWorks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TestTools;

namespace AutoSFCTools
{
    class IPworks_Telnet
    {
        public Telnet _telnet = new Telnet();
        public string dateRe = "";

        public IPworks_Telnet()
        {
           
            this._telnet.OnConnected += new Telnet.OnConnectedHandler(telnet_OnConnected);
            this._telnet.OnDataIn += new Telnet.OnDataInHandler(telnet_OnDataIn);
            this._telnet.OnDo += new Telnet.OnDoHandler(telnet_OnDo);
            this._telnet.OnWill += new Telnet.OnWillHandler(telnet_OnWill);
        }

        void telnet_OnWill(object sender, TelnetWillEventArgs e)
        {
            try
            {
                if (e.OptionCode == 38)
                    _telnet.DontOption = 38;
            }
            catch (IPWorksException)
            {
            }
        }

        void telnet_OnDo(object sender, TelnetDoEventArgs e)
        {
            try
            {
                _telnet.WontOption = e.OptionCode;
            }
            catch (IPWorksException)
            {
            }
        }

        void telnet_OnDataIn(object sender, TelnetDataInEventArgs e)
        {
            dateRe += e.Text;
        }

        void telnet_OnConnected(object sender, TelnetConnectedEventArgs e)
        {
            _telnet.AcceptData = true;
        }

        public int Write(string strSend)
        {
            _telnet.DataToSend = strSend + "\r\n";
            return 0;
        }

        public string Read()
        {
            return dateRe;
        }
        public int SendReceive(string strSend, string strToLookFor, int iTimeout, out string strReceived)
        {
             sDelay.Delay(1000);
             dateRe = "";
             _telnet.DataToSend = strSend + "\r\n";
             while (iTimeout > 0)
             {
                 if (dateRe.IndexOf(strToLookFor) >= 0)
                 {
                     strReceived = dateRe;
                     return 0;
                 }
                 else
                 {
                     sDelay.Delay(1000);
                     iTimeout -= 1;
                 }

             }
              strReceived = dateRe;
              return -1;          
        }

    }
}
