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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            RegisterMove();
            this.Load += Login_Load;
        }

        void Login_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = GetMyselfIP();                                        //获取本机ip
            this.textBox2.Text = "45554";                                              //所有客户端默认端口 45554
        }

        private void registerId_Click(object sender, EventArgs e)
        {
            RegisterInfo rinfo = new RegisterInfo();
            rinfo.Show(this);
        }

        private void loginIp_Click(object sender, EventArgs e)
        {
            if (CheckInputRule() == false)
            {
                return;
            }
            List<Object> listInfo = new List<object>();
            listInfo.Add("3");                  //代表是登陆中&请求登陆
            listInfo.Add(this.textBox1.Text);
            listInfo.Add(this.textBox2.Text);
            listInfo.Add(this.textBox3.Text);
            TcpSkt tcpskt = new TcpSkt();
            List<Object> listInfo2 = tcpskt.LR_ActionSend_Receive(listInfo);
            if (listInfo2 == null)
            {
                MessageBox.Show("连接服务器失败，可能是服务端未开启，请联系管理员", "提示",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                return;
            }
            if (listInfo2[0].ToString() == "4")     //相等时候登陆成功
            {
                //从服务器取得信息 到客户端 （其他客户端信息） 传给主界面
                listInfo2.Add(this.textBox1.Text);//自己的ip
                listInfo2.Add(this.textBox2.Text);//自己的port
                this.Owner.Tag = listInfo2;
                this.Close();
            }
            else 
            {
                MessageBox.Show("登陆失败", "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Owner.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Owner.Close();
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
            if (!ipR.IsMatch(ip))
            {
                toolTip1.Show("不符合格式请按照ip格式输入！\n___.___.___.___", textBox1);
                textBox1.Focus();
                return false;
            }
            if (!portR.IsMatch(port))
            {
                toolTip1.Show("不符合格式请按照port格式输入！\n 1-65535（未使用的端口）", textBox2);
                textBox2.Focus();
                return false;
            }
            if (!pwdR.IsMatch(pwd))
            {
                toolTip1.Show("不符合格式请按照密码格式输入！\n只可输入_数字和字母\n在6-16以内", textBox3);
                textBox3.Focus();
                return false;
            }
            return true;
        }

        #region 拖动窗体

        void RegisterMove()
        {
            pictureBox1.MouseDown+=pictureBox1_MouseDown;
            pictureBox1.MouseMove+=pictureBox1_MouseMove;
            pictureBox1.MouseUp+=pictureBox1_MouseUp;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Tag = MousePosition.X + " " + MousePosition.Y + " true";
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pictureBox1.Tag = MousePosition.X + " " + MousePosition.Y + " false";
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string str = (string)pictureBox1.Tag;
                string[] strinfo = str.Split(' ').ToArray();
                if (Convert.ToBoolean(strinfo[2]))
                {
                    this.Left += MousePosition.X - Convert.ToInt32(strinfo[0]);
                    this.Top += MousePosition.Y - Convert.ToInt32(strinfo[1]);
                    pictureBox1.Tag = MousePosition.X + " " + MousePosition.Y + " true";
                }
            }
        }
        #endregion
    }
}
