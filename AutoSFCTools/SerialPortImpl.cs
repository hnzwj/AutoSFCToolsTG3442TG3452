using SFCTools;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace AutoSFCTools
{

    public class SerialPortImpl
    {
        private SerialPort _serialPort;
        private string _receivedData;
        private AutoResetEvent dataEvent;
        private string _sPort;
        private string _sBaudRate;
        private Parity _parity;
        private int _dataBits;
        private StopBits _stopBits;

        public SerialPortImpl()
        {
            this.dataEvent = new AutoResetEvent(false);
        }
        ~SerialPortImpl()
        {

        }

        public bool IsOpen()
        {
            return this._serialPort.IsOpen;
        }
        public void Open()
        {
            this.Open(this._sPort, this._sBaudRate, this._parity, this._dataBits, this._stopBits);
        }
        public void Open(string sPort, string sBaudRate)
        {
            this.Open(sPort, sBaudRate, Parity.None, 8, StopBits.One);
        }
        public void Open(string sPort, string sBaudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this._sPort = sPort;
            this._sBaudRate = sBaudRate;
            this._parity = parity;
            this._dataBits = dataBits;
            this._stopBits = stopBits;
            try
            {
                this._serialPort = new SerialPort(this._sPort, Convert.ToInt32(this._sBaudRate), this._parity, this._dataBits, this._stopBits);
                this._serialPort.Handshake = Handshake.None;
                this._serialPort.DataReceived += new SerialDataReceivedEventHandler(this.DataReceived);
                if (!this._serialPort.IsOpen)
                {
                    this._serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                var msg = "Open() exception: " + ex.Message;
                LogHelper.Info(msg);
            }
        }

        public void Close()
        {
            if (this._serialPort == null)
            {
                return;
            }
            if (this._serialPort.IsOpen)
            {
                this._serialPort.Close();
                return;
            }
        }

        public void SendData(string sCommand)
        {
            this._receivedData = string.Empty;
            try
            {
                this._serialPort.WriteLine(sCommand);

            }
            catch (Exception ex)
            {
                var msg = "SendData() exception: " + ex.Message;
                LogHelper.Info(msg);
            }
        }
        public string ReceiveData()
        {
            return this._receivedData;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = this._serialPort.BytesToRead;
            byte[] array = new byte[bytesToRead];
            if (this._serialPort.Read(array, 0, bytesToRead) == 0)
            {
                return;
            }
            this._receivedData += Encoding.ASCII.GetString(array);

            if (this._receivedData.Length > 3)
            {
                this.dataEvent.Set();
                Thread.Sleep(50);
                return;
            }
            this._receivedData = string.Empty;
        }

        public void WaitForDataAvailable(int timeoutMS)
        {
            this._receivedData = string.Empty;
            this.dataEvent.Reset();
            this.dataEvent.WaitOne(timeoutMS);
        }
        public void Stop()
        {
            this.dataEvent.Set();
            //this.log.Debug("Stop()");
            Thread.Sleep(50);
        }
    }
}
