using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace TestTools
{
    class MySqlDBHelper
    {
        private LINQToINI Ini = new LINQToINI();
        private string sConfigPath = "";
        private MySqlConnection dbconn()
        {
            this.sConfigPath = Environment.CurrentDirectory + "\\config.cfg";
            Ini.Load(sConfigPath);
            string ipHost = Ini.GetProfileAsString("DB", "host", "127.0.0.1");
            string User = Ini.GetProfileAsString("DB", "user", "te");
            string pwd = Ini.GetProfileAsString("DB", "pwd", "te123.");
            string db = Ini.GetProfileAsString("DB", "db", "tg2482_auto");
            string strConnection = string.Format("server={0};User Id={1};database={2};password={3};",ipHost,User,db,pwd);
            MySqlConnection conn = new MySqlConnection(strConnection);
            return conn;
        }
        /// <summary>
        /// 查询数据库信息，返回Datatable
        /// </summary>
        /// <param name="sqlRec"></param>
        /// <returns></returns>
        public DataTable selectInfo(string sqlRec)
        {
            string sql = sqlRec;
            MySqlConnection dtconn = new MySqlConnection();
            DataTable dt= new DataTable();
            dtconn = dbconn();
            try
            {
                if (dtconn.State == ConnectionState.Closed)
                {
                    dtconn.Open();
                }
                MySqlCommand cmd = new MySqlCommand(sql, dtconn);
                MySqlDataAdapter da = new MySqlDataAdapter(sql, dtconn);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dtconn.Close();
            }
            return dt;
        }
        /// <summary>
        /// 返回reader
        /// </summary>
        /// <param name="recStr"></param>
        /// <returns></returns>
        public MySqlDataReader dbreader(string recStr)
        {
            MySqlConnection dtconn = new MySqlConnection();
            dtconn = dbconn();
            MySqlCommand cmd = new MySqlCommand(recStr, dtconn);
            MySqlDataReader reader = null;
            try
            {
                if (dtconn.State == ConnectionState.Closed)
                {
                    dtconn.Open();
                    reader = cmd.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //finally
            //{
            //    dtconn.Close();
            //}
            return reader;
        }
        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="sqlRec"></param>
        /// <returns>object</returns>
        public object selectResult(string sqlRec)
        {
            object n = null; //= new object();
            string sql = sqlRec;
            MySqlConnection dtconn = new MySqlConnection();
            dtconn = dbconn();
            try
            {
                dtconn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, dtconn);
                n = cmd.ExecuteScalar();//返回首行首列
                //i = Convert.ToInt32(n);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dtconn.Close();
            }
            return n;
        }
        /// <summary>
        /// 更新及插入结果
        /// </summary>
        /// <param name="sqlRec"></param>
        /// <returns></returns>
        public int updataResult(string sqlRec)
        {
            int i = 0;
            string sql = sqlRec;
            MySqlConnection dtconn = new MySqlConnection();
            dtconn = dbconn();
            try
            {
                dtconn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, dtconn);
                i = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dtconn.Close();
            }
            return i;
        }
    }
}
