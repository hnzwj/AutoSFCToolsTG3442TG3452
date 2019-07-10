using System;
using System.Windows.Forms;
using TestTools;

namespace AutoSFCTools
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        private LINQToINI Ini = new LINQToINI();
        private string sConfigPath = "";
        public bool closeflag = true;
       
        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.sConfigPath = Environment.CurrentDirectory + "\\config.cfg";
                Ini.Load(sConfigPath);

                StationInfo.StationID = Ini.GetProfileAsString("Settings", "name", "NULL");
                StationInfo.UserID = Ini.GetProfileAsString("Settings", "sfcname", "NULL");
                StationInfo.IcamPort = Ini.GetProfileAsInt("Settings", "com", 3);
                StationInfo.Fixture = Ini.GetProfileAsString("Settings", "Fixture", "0");
                StationInfo.TotalUUT = Ini.GetProfileAsInt("Settings", "Qty", 4);
                StationInfo.Robot = Ini.GetProfileAsBool("Settings", "Robot", false);
                StationInfo.stationNumber = Ini.GetProfileAsString("Settings", "StationNumber", "0");

                if (StationInfo.StationID.ToUpper() == "AFTSII")
                {
                    StationInfo.TelnetLogPath = @"C:\ARRIS\AFTSII\RawTestLog";
                    StationInfo.TestReport = @"C:\ARRIS\AFTSII\TestReports";
                }
                if (StationInfo.StationID.ToUpper() == "ARFTS")
                {
                    StationInfo.TelnetLogPath = @"C:\ARRIS\ARFTS\RawTestLog";
                    StationInfo.TestReport = @"C:\ARRIS\ARFTS\TestReports";
                }
                if (StationInfo.StationID.ToUpper() == "FTTS")
                {
                    StationInfo.TelnetLogPath = @"C:\DOCSIS3.1 TRAFFIC TEST\Telnet Log";
                    StationInfo.ResultLogPath = @"C:\\DOCSIS3.1 TRAFFIC TEST\\Result";
                }
                //if (!Directory.Exists(testLogPath))
                //{
                //    MessageBox.Show("Can not found " + testLogPath, "Station Error ?");
                //    this.Dispose();
                //}

                string[] IDs = new string[] { StationInfo.StationID, "F1323402" };

                comboBox1.Items.AddRange(IDs);
                comboBox1.SelectedIndex = 0;
                this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                string ver = getSwver();
                if (0 != CheckVer(ver))
                {
                    MessageBox.Show("测试软件版本错误，请联系TE", "Tips");
                    this.Dispose(true);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private int CheckVer(string ver)
        {
            //DBHelper db = new DBHelper();
            int right = 0;
            //if (0 != db.CheckTestToolVer("TestTools", ver, out right))
            //{
            //    MessageBox.Show("查询数据库版本信息错误，请联系TE");
            //}
            return right;
        }
        private string getSwver()
        {
            //string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString();
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string time = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location).ToString();
            string _appVersion = ver.ToString();
            ver += " Date: " + time;
            this.Text = string.Format("{0}:{1}", " SFC tools by wj Version ", ver);
            return _appVersion;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == StationInfo.StationID)  //直接启动
            {
                Form1 main = new Form1(this);
                main.Show();
            }
            if (comboBox1.SelectedItem.ToString() == "F1323402")  //管理员模式
            {
                MessageBox.Show("Admin model");
            }
        }
    }
}
