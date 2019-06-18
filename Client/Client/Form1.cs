using Client.AboutSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        public string Ip { get; set; }          //本机ip
        public string Port { get; set; }        //本机port
        public void TransInfo(string ip,string port)
        {
            Ip = ip;
            Port = port;
            ipInfo.Text = ip;
            portInfo.Text = port;
        }   //给Ip Port 和对应标签赋值 

        //-----------------------------------------
        //刷新好友列表的时间间隔
        public int IntervalUpdate { get; set; }
        //-----------------------------------------

        #region 用于获取用户详细信息后进行存储

        //存储自己的信息
        List<Object> UserInfo = null;
        //存储查找信息的时候需要的ip 与 port
        public string IpDetail { get; set; }
        public string PortDetail { get; set; }  

        #endregion
      

        public ClientForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += ClientForm_Load;
            this.FormClosed += ClientForm_FormClosed;
            IntervalUpdate = 1000 * 60;             //一分钟更新一次好友列表
        }

        #region 事件处理
        
        void ClientForm_Load(object sender, EventArgs e)
        {
            Login login = new Login();
            login.ShowDialog(this);
            //基础信息显示 ip port 好友列表
            BasicInfoListView(listView1);

            timer1.Interval = IntervalUpdate;
            timer1.Enabled = true;
            //显示Icon信息 的初始化信息
            notifyIcon1Basic();
        }

        #region 用来更新好友列表

        //用来更新好友列表
        private void timer1_Tick(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(UpdateTheFriendList));
            thread.IsBackground = true;
            thread.Start();
        }

        private void UpdateTheFriendList()
        {
            List<Object> listInfo = new List<object>();
            listInfo.Add("7");                  //代表是更新用户列表
            listInfo.Add(Ip);
            listInfo.Add(Port); 
            TcpSkt tcpskt = new TcpSkt();
            List<Object> listInfo2 = tcpskt.LR_ActionSend_Receive(listInfo);
            if (listInfo2 == null)
            {
                MessageBox.Show("连接服务器失败，可能是服务端未开启，请联系管理员", "提示",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                return;
            }
            if (listInfo2[0].ToString() == "7")     //相等时候更新成功
            {
                //从服务器取得信息 到客户端 （其他客户端信息） 传给主界面
                listView1.Invoke((MethodInvoker)(() =>          //更新服务器的显示信息
                {
                    listInfo2.Add(Ip);
                    listInfo2.Add(Port);
                    listView1.Items.Clear();
                    FullListViewAndShowInfo(listView1, listInfo2);
                }));
            }
            else
            {
                MessageBox.Show("更新失败", "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        #endregion

        //关闭时 让服务器改变登陆状态
        void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //让服务器改变登陆状态
            List<Object> listInfo = new List<object>();
            listInfo.Add("12");                         //代表 客户端下线
            listInfo.Add(Ip);
            listInfo.Add(Port);
            if (Ip == null)                             //判断是否是在进入主窗体后退出 Ip==null 则是在登陆前gg
                return;
            TcpSkt tcpskt = new TcpSkt();
            List<Object> listInfo2 = tcpskt.LR_ActionSend_Receive(listInfo);
            if (listInfo2 == null)
            {
                MessageBox.Show("连接服务器失败，可能是服务端未开启，请联系管理员", "提示",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                return;
            }
            if (listInfo2[0].ToString() == "12")     //相等时候退出成功
            {
                MessageBox.Show("已退出", "提示",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("退出出现错误", "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Owner.Close();
            }
        }

        #region 代表是获取用户信息

        //点击头像 显示详细信息
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            IpDetail = Ip;
            PortDetail = Port;
            GetUserInfo2();
            UserDetailInfo udi = new UserDetailInfo();
            udi.Tag = UserInfo;
            udi.Show(this);
        }

        //代表是获取用户信息
        void GetUserInfo2()
        {
            List<Object> listInfo = new List<object>();
            listInfo.Add("22");                  //代表是获取用户信息
            listInfo.Add(IpDetail);
            listInfo.Add(PortDetail);
            TcpSkt tcpskt = new TcpSkt();
            List<Object> listInfo2 = tcpskt.LR_ActionSend_Receive(listInfo);
            if (listInfo2 == null)
            {
                MessageBox.Show("连接服务器失败，可能是服务端未开启，请联系管理员", "提示",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                return;
            }
            if (listInfo2[0].ToString() == "22")     //相等时候获取成功
            {
                //从服务器取得user信息 到客户端 （其他客户端信息） 传给主界面
                UserInfo = new List<object>();
                for (int s = 1; s < 9;s++ )
                    UserInfo.Add(listInfo2[s].ToString());
            }
            else
            {
                MessageBox.Show("更新失败", "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }


        #endregion

        #endregion

        #region listView的初始化

        /// <summary>
        /// listView的初始化
        /// </summary>
        private void BasicInfoListView(ListView listV)
        {            
            listV.MultiSelect = false;  //不可选多行
            listV.View = View.Details;
            listV.GridLines = true;//网格线
            listV.FullRowSelect = true;//选全行
            listV.HeaderStyle = ColumnHeaderStyle.Clickable;
            listV.Columns.Add("name", 70);
            listV.Columns.Add("ip", 90);
            listV.Columns.Add("port", 60);
            listV.Columns.Add("islogin", 54);

            //右键菜单
            ContextMenu cms = new ContextMenu();
            MenuItem it1 = new MenuItem("开始聊天");
            MenuItem it2 = new MenuItem("好友信息");
            cms.MenuItems.Add(it1);
            cms.MenuItems.Add(it2);
            it1.Click += it1_Click;
            it2.Click += it2_Click;

            listV.ContextMenu = cms;
            if (this.Tag == null)           //防止空引用发生
                return;
            List<Object> listOtherClientinfo = this.Tag as List<Object>;

            FullListViewAndShowInfo(listV, listOtherClientinfo); //获取客户端列表 显示在ListView
            
        }
        //打开好友信息窗口
        private void it2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;
            ListViewItem ivi = listView1.SelectedItems[0];
            
            IpDetail = ivi.SubItems[1].Text.ToString();
            PortDetail = ivi.SubItems[2].Text.ToString();

            GetUserInfo2();
            UserDetailInfo udi = new UserDetailInfo();
            udi.Tag = UserInfo;
            udi.Show(this);
        }
        //打开聊天窗口
        private void it1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;
            List<Object> listTwoInfo = new List<object>();
            listTwoInfo.Add(Ip);//自己的ip
            listTwoInfo.Add(Port);//自己的port
            listTwoInfo.Add(listView1.SelectedItems[0].SubItems[1].Text.ToString());//对方ip
            listTwoInfo.Add(listView1.SelectedItems[0].SubItems[2].Text.ToString());//对方port
            ChatForm chatform = new ChatForm();
            chatform.Tag = listTwoInfo;
            chatform.Show(this);
        }

        //获取客户端列表  //调用给Ip Port 和对应标签赋值的函数
        private void FullListViewAndShowInfo(ListView listV, List<Object> listOtherClientinfo)
        {            
            for (int i = 1; i <= (listOtherClientinfo.Count - 1) / 4; i++)     //获取客户端列表
            {
                int t = 4 * (i - 1);
                if (t == 0)
                {
                    ListViewItem ivi = new ListViewItem(listOtherClientinfo[1].ToString());//name
                    ivi.SubItems.Add(listOtherClientinfo[2].ToString());//ip
                    ivi.SubItems.Add(listOtherClientinfo[3].ToString());//port
                    ivi.SubItems.Add(listOtherClientinfo[4].ToString());//islogin
                    listV.Items.Add(ivi);
                }
                else
                {
                    ListViewItem ivi2 = new ListViewItem(listOtherClientinfo[t + 1].ToString());//name
                    ivi2.SubItems.Add(listOtherClientinfo[t + 2].ToString());//ip
                    ivi2.SubItems.Add(listOtherClientinfo[t + 3].ToString());//port
                    ivi2.SubItems.Add(listOtherClientinfo[t + 4].ToString());//islogin
                    listV.Items.Add(ivi2);
                }
            }
            TransInfo(listOtherClientinfo[listOtherClientinfo.Count - 2].ToString(),
                listOtherClientinfo[listOtherClientinfo.Count - 1].ToString());   //获取本机ip port
        }

        #endregion

        #region 显示Icon信息

        private void notifyIcon1Basic()
        {
            ContextMenuStrip cms = new ContextMenuStrip ();
            ToolStripMenuItem cms1 = new ToolStripMenuItem("打开WoW主界面");
            ToolStripMenuItem cms2 = new ToolStripMenuItem("退出WoW");
            cms.Items.Add(cms1);
            cms.Items.Add(cms2);
            cms1.Click += cms1_Click;
            cms2.Click += cms2_Click;
            notifyIcon1.ContextMenuStrip = cms;
        }

        void cms2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void cms1_Click(object sender, EventArgs e)
        {
            ShowTheMainForm();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowTheMainForm();
        }

        void ShowTheMainForm()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }


        #endregion


    }
}
