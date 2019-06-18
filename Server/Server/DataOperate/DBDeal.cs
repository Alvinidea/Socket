using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Windows.Forms;
using System.Data;

namespace Server.DataOperate
{
    class DBDeal
    {
        /// <summary>
        /// DataBase 的连接变量
        /// </summary>
        SqlConnection conn;
        /// <summary>
        /// 用于返回连接对象
        /// </summary>
        /// <returns></returns>
        public SqlConnection SqlConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["SQLconnStr"].ToString();
            return new SqlConnection(connStr);
        }
        /// <summary>
        /// 对数据库执行NonQuery操作
        /// </summary>
        /// <param name="insertStr"></param>
        /// <returns></returns>
        public int SqlAction(string insertStr)
        {
            conn = SqlConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(insertStr, conn);
            int i = cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Dispose();
            return i;
        }
        /// <summary>
        /// 数据填充 DataGridView
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="selectStr"></param>
        public void SqlFullDataGridView(DataGridView dgv, string selectStr)
        {
            conn = SqlConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(selectStr, conn);
            SqlDataAdapter oda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            oda.Fill(ds);
            dgv.DataSource = ds.Tables[0];
            cmd.Dispose();
            conn.Dispose();
        }
        /// <summary>
        /// 判断数据库是否有信息
        /// </summary>
        /// <param name="selectStr"></param>
        /// <returns></returns>
        public bool SqlSelectHas(string selectStr)
        {
            conn = SqlConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(selectStr, conn);
            SqlDataReader sdr = cmd.ExecuteReader();
            bool bo = sdr.HasRows;
            sdr.Dispose();
            cmd.Dispose();
            conn.Dispose();
            return bo;
        }
        /// <summary>
        /// 读取信息 并返回SqlDataReader对象
        /// </summary>
        /// <param name="selectStr"></param>
        /// <returns></returns>
        public SqlDataReader SqlGetThreeInfo(string selectStr)
        {
            conn = SqlConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(selectStr, conn);
            SqlDataReader odr = cmd.ExecuteReader();
            return odr;
        }
        /// <summary>
        /// 释放 连接 和public SqlDataReader SqlTimrGetInfo(string selectStr)一起用
        /// </summary>
        public void ShutDown()
        {
            conn.Dispose();
        }
    }
}
