using Client.AboutSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class RegisterInfo : Form
    {
        public RegisterInfo()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += RegisterInfo_Load;
            button1.Click += button1_Click;
        }

        void RegisterInfo_Load(object sender, EventArgs e)
        {            
            this.textBox1.Text = GetMyselfIP(); //获取本机ip
            this.textBox2.Text = "45554";       //所有客户端默认端口 45556
            sex1.Checked = true;        
        }

        void button1_Click(object sender, EventArgs e)
        {
            if (CheckInputRule() == false)
            {
                return;
            }
            List<Object> listInfo = new List<object> ();
            listInfo.Add("1");                  //代表是注册
            listInfo.Add(this.textBox1.Text);
            listInfo.Add(this.textBox2.Text);
            listInfo.Add(this.textBox3.Text);
            listInfo.Add(this.textBox4.Text);
            listInfo.Add(GetSex());
            TcpSkt tcpskt = new TcpSkt();
            List<Object> listInfo2 = tcpskt.LR_ActionSend_Receive(listInfo);    //发送注册信息给服务端
            if (listInfo2 == null)
            {
                MessageBox.Show("连接服务器失败，可能是服务端未开启，请联系管理员", "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            if (listInfo2[0].ToString() == "2")
            {
                MessageBox.Show("注册成功","提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            if (listInfo2[0].ToString() == "23")
            {
                MessageBox.Show("（23）用户已存在\n请用其他的ip port注册!!!", "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //获取本机IP
        string GetMyselfIP()
        {
            string str = String.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    str = _IPAddress.ToString();
                }
            }
            return str;
        }

        //检查输入规则
        bool CheckInputRule()
        {
            string ip = textBox1.Text;
            string port = textBox2.Text;
            string pwd = textBox3.Text;
            Regex ipR = new Regex(@"[0-9]{1,3}(\.[0-9]{1,3}){3}");
            Regex portR = new Regex(@"[0-9]{1,5}");
            Regex pwdR = new Regex(@"^[a-zA-Z0-9_]{6,16}$");
            if ( !ipR.IsMatch(ip))
            {
                toolTip1.Show("不符合格式请按照ip格式输入！\n___.___.___.___",textBox1);
                textBox1.Focus();
                return false;
            }
            if ( !portR.IsMatch(port))
            {
                toolTip1.Show("不符合格式请按照port格式输入！\n 1-65535（未使用的端口）", textBox2);
                textBox2.Focus();
                return false;
            }
            if ( !pwdR.IsMatch(pwd))
            {
                toolTip1.Show("不符合格式请按照密码格式输入！\n只可输入_数字和字母\n在6-16以内", textBox3);
                textBox3.Focus();
                return false;
            }
            return true;
        }
        //获取选择的性别
        string GetSex()
        {
            if (sex1.Checked)
            {
                return "1";
            }
            else 
            {
                return "0";
            }
        }
    }
}
