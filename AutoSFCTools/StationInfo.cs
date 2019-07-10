using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSFCTools
{
    public static class StationInfo
    {
        public static string TelnetLogPath
        {
            get;
            set;
        }
        public static string stationNumber
        {
            get;
            set;
        }
        public static int LogCount
        {
            get;
            set;
        }
        public static string TestReport
        {
            get;
            set;
        }
        public static string ResultLogPath
        {
            get;
            set;
        }
        public static bool FTTS_test
        {
            get;
            set;
        }
        public static bool ARFTS_AFTSII_test
        {
            get;
            set;
        }
        public static bool Automation
        {
            get;
            set;
        }
        public static bool Robot
        {
            get;
            set;
        }
        public static string StationID
        {
            get;
            set;
        }
        public static string UserID
        {
            get;
            set;
        }
        public static string Uut1Host
        {
            get;
            set;
        }
        public static string Uut2Host
        {
            get;
            set;
        }
        public static string Uut3Host
        {
            get;
            set;
        }
        public static string Uut4Host
        {
            get;
            set;
        }

        public static bool ICAM
        {
            get;
            set;
        }
        public static int IcamPort
        {
            get;
            set;
        }
        public static string Fixture
        {
            get;
            set;
        }
        public static string DmmAddress
        {
            get;
            set;
        }
        public static string PsuAddress
        {
            get;
            set;
        }
        public static int CameraPort
        {
            get;
            set;
        }
        public static int Telephone
        {
            get;
            set;
        }

        public static int TotalUUT
        {
            get;
            set;
        }
        public static int FLAG
        {
            get;
            set;
        }
    }
}
