using Server.DataOperate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class ServerForm : Form
    {
        #region 服务器参数


        string strIP = "127.0.0.1";//Dns.GetHostEntry(Dns.GetHostName()).AddressList[2].ToString();//"127.0.0.1";     //服务器IP(运行的ip)
        int strPort = 45555;            //服务器port
        bool sign = true;               //暂时没有用
        Socket server = null;           //服务器Socket
        public int ThreadCount { get; set; }        //线程数

        //获取本机IP
        string GetServerIP()
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

        #endregion

        public ServerForm()
        {
            //线程数
            ThreadCount = 500;
            //获取本机ip充当 Server端IP    在本机测试时候可以注释掉
            strIP = GetServerIP();

            InitializeComponent();
            //注册事件
            RegisterEvent();

            BeauityForm();
            //dataGridView的右键菜单
            ClientContextMenuStrip();
        }

        #region 初始化事件 界面美化
        //注册事件
        void RegisterEvent()
        {
            this.Load += ServerForm_Load;
            开启服务ToolStripMenuItem.Click += 开启服务ToolStripMenuItem_Click;
            关闭服务ToolStripMenuItem.Click += 关闭服务ToolStripMenuItem_Click;
        }
        //美化界面
        void BeauityForm()
        {
            dataGridView1.BackgroundColor = dataGridView1.Parent.BackColor;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;

            label1.Text = strIP;
            label2.Text = strPort.ToString(); 
        }

        #endregion

        #region 事件处理模块

        //登陆事件
        void ServerForm_Load(object sender, EventArgs e)
        {
            ShowClientInfo();
        }

        void 开启服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowClientInfo();
            ToolStripMenuItemSign(true);
            OpenServer3();
        }

        private void 关闭服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowClientInfo();
            ToolStripMenuItemSign(false);
            CloseServer();
        }
        //这是两个按钮的 标记
        void ToolStripMenuItemSign(bool i)
        {
            开启服务ToolStripMenuItem.Checked = i;
            关闭服务ToolStripMenuItem.Checked = !i;
            开启服务ToolStripMenuItem.Enabled = !i;
            关闭服务ToolStripMenuItem.Enabled = i;
        }
        #endregion

        #region 用来临时保存信息的参数
        /*
         * 22 号信息的时候 用来保存client传来的ip 和 port
         * 
         */
        List<Object> temp = null;

        #endregion

        #region 打开关闭服务事件中的处理函数

        //------------------------------------------------------------------------------------------

        //打开服务 淘汰版
        private void OpenServer()
        {
            IPAddress localIP = IPAddress.Parse(strIP);
            IPEndPoint ipp = new IPEndPoint(localIP, strPort);

            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            server.Bind(ipp);       //本地地址绑定到套接字
            server.Listen(20);      //开始监听   挂起连接队列的最大长度 20
            richTextBox1.Text = "服务器开启监听！！！";

            //================================================================================
            Socket clientConn = server.Accept();
            sign = true;
            if (clientConn.Connected == true)
            {
                richTextBox1.Text += "\n一个客户端连入";
                List<Object> info = null;
                int i = 1;                                      //暂时的定义循环次数
                while (sign)
                {
                    Byte[] byteNum2 = new Byte[1024];

                    clientConn.Receive(byteNum2, byteNum2.Length, 0);               //接收数据  
                   
                    info = (ClassSerializers.DeSerializebinary(new MemoryStream(byteNum2) as MemoryStream)) as List<Object>;
                                                                                    //信息提取  反序列化
                    List<Object> infolist = new List<object> ();    //用来存储客户端发来的信息
                    infolist.Add( IsLoginOrRegister(info) );        //代表注册或登陆成功 数据存储or数据对比

                    infolist = SendAboutLoOrRe(infolist);           //将需要发送给客户端的信息发送给客户端

                    byte[] data = ClassSerializers.Serializebinary(infolist).ToArray(); // 将信息序列化为byte[]
                    clientConn.Send(data, data.Length, 0);          //发送信息
                    if ( --i == 0)
                        sign = false;                               //结束循环
                }
                richTextBox1.Text += "\n一个客户端关闭连接";
            }
        }
        //打开服务 更新版
        private void OpenServer2()
        {
            IPAddress localIP = IPAddress.Parse(strIP);
            IPEndPoint ipp = new IPEndPoint(localIP, strPort);

            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            server.Bind(ipp);       //本地地址绑定到套接字
            server.Listen(20);      //开始监听   挂起连接队列的最大长度 20
            richTextBox1.Text = "服务器开启监听！！！";
            //while (true)

            for (int o = 0; o <= ThreadCount; o++)
            {
                Thread serverthread = new Thread(
                    () =>
                    {
                        Invoke((MethodInvoker)
                        //BeginInvoke((MethodInvoker)
                            (() =>
                            {
                                Socket clientConn = server.Accept();
                                sign = true;
                                if (clientConn.Connected == true)
                                {
                                    richTextBox1.Text += "\n一个客户端连入";
                                    List<Object> info = null;
                                    int i = 1;                                      //暂时的定义循环次数
                                    while (sign)
                                    {
                                        Byte[] byteNum2 = new Byte[1024];

                                        clientConn.Receive(byteNum2, byteNum2.Length, 0);               //接收数据  

                                        info = (ClassSerializers.DeSerializebinary(new MemoryStream(byteNum2) as MemoryStream)) as List<Object>;
                                        //信息提取  反序列化
                                        List<Object> infolist = new List<object>();    //用来存储客户端发来的信息
                                        infolist.Add(IsLoginOrRegister(info));        //代表注册或登陆成功  数据存储or数据对比  还有下线

                                        infolist = SendAboutLoOrRe(infolist);           //将需要发送给客户端的信息发送给客户端

                                        byte[] data = ClassSerializers.Serializebinary(infolist).ToArray(); // 将信息序列化为byte[]
                                        clientConn.Send(data, data.Length, 0);          //发送信息
                                        if (--i == 0)
                                            sign = false;                               //结束循环
                                    }
                                    richTextBox1.Text += "\n一个客户端关闭连接";
                                }
                            })
                            );
                    });
                serverthread.IsBackground = true;
                serverthread.Start();
            }
        }
        //打开服务 更新版2     (更新版 invoke的位置不对） 
        private void OpenServer3()
        {
            IPAddress localIP = IPAddress.Parse(strIP);
            IPEndPoint ipp = new IPEndPoint(localIP, strPort);

            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            server.Bind(ipp);       //本地地址绑定到套接字
            server.Listen(20);      //开始监听   挂起连接队列的最大长度 20
            richTextBox1.Text += "服务器开启监听！！！";

            for (int o = 0; o <= ThreadCount; o++)
            {
                Thread serverthread = new Thread( new ThreadStart(TheServerThread));
                serverthread.IsBackground = true;
                serverthread.Start();
            }
        }
        //打开服务 更新版2 需要调用的函数
        private void TheServerThread()
        {
            try
            {
                Socket clientConn = server.Accept();
                sign = true;
                if (clientConn.Connected == true)
                {
                    OperateAboutUIControl_RichTextBox(richTextBox1, "客户端连入------");
                    List<Object> info = null;
                    int i = 1;                                      //暂时的定义循环次数
                    while (sign)
                    {
                        Byte[] byteNum2 = new Byte[1024];

                        clientConn.Receive(byteNum2, byteNum2.Length, 0);               //接收数据  

                        info = (ClassSerializers.DeSerializebinary(new MemoryStream(byteNum2) as MemoryStream)) as List<Object>;
                        //信息提取  反序列化
                        List<Object> infolist = new List<object>();    //用来存储客户端发来的信息

                        infolist.Add(IsLoginOrRegister(info));        //代表注册或登陆成功  数据存储or数据对比  还有下线

                        infolist = SendAboutLoOrRe(infolist);           //将需要发送给客户端的信息发送给客户端

                        byte[] data = ClassSerializers.Serializebinary(infolist).ToArray(); // 将信息序列化为byte[]
                        clientConn.Send(data, data.Length, 0);          //发送信息
                        if (--i == 0)
                            sign = false;                               //结束循环
                    }
                    OperateAboutUIControl_RichTextBox(richTextBox1, "客户端退出------");
                }
            }
            catch 
            {
                return;
            }
        }

        //------------------------------------------------------------------------------------------

        //判断是登陆还是注册 用户列表 退出 用户信息
        private string IsLoginOrRegister(List<Object> info) 
        {
            ShowInfoOnView(info, info[0].ToString());
            switch (info[0].ToString())
            {
                case "1":
                    //是注册的话先判断是否存在   不存在则新建并进行数据存储
                    if ( IsExistID(info) == false )
                    {
                        string insertStr = String.Format("insert into IDInfo(ip,port,pwd,name,sex,lastlogindate,islogin) values('{0}',{1},'{2}','{3}','{4}','{5}',{6})",
                            info[1].ToString(),
                            Convert.ToInt32(info[2].ToString()),
                            info[3].ToString(),
                            info[4].ToString(),
                            info[5].ToString(),
                            DateTime.Now.ToString(),
                            0); //0 代表下线  1 代表上线
                        DBDeal dbDeal = new DBDeal();
                        int i = dbDeal.SqlAction(insertStr);
                        if (i > 0)
                        {
                            //更新服务器的显示信息
                            dataGridView1.Invoke((MethodInvoker)(() =>
                            {
                                ShowClientInfo();
                            }));
                            //在注册时候会出现错误  会直接进入下一线程    交给ui线程 在等ui线程？？ 都在等（死锁）
                            //MessageBox.Show("注册成功！！！", "提示",
                            //    MessageBoxButtons.OK,
                            //    MessageBoxIcon.Information);
                        }
                        return "2";
                    }
                    else
                    {
                        return "23";
                    }
                case "3":
                    //是登陆的话进行数据对比
                    string selectstr = String.Format("select * from IDInfo where ip='{0}' and port = {1} and pwd='{2}'",
                        info[1].ToString(),
                        Convert.ToInt32(info[2].ToString()),
                        info[3], ToString());
                    DBDeal dbDeal2 = new DBDeal();
                    if (dbDeal2.SqlSelectHas(selectstr))                //有账号则返回客户端  "4"
                    {
                        UpdateLoginStateAndTime_Close(info, 1,1);       //修改该账号的登陆状态到数据库

                        dataGridView1.Invoke((MethodInvoker)(() =>      //更新服务器的显示信息
                        {
                            ShowClientInfo();
                        }));
                        return "4";
                    }
                    return "gg";                                        //无账号则返回客户端  "gg"
                case "7":                                              
                    //进行客户端好友列表更新                  
                    return "7";
                case "12":                                              //代表下线
                    //是退出的话进行数据更新
                    UpdateLoginStateAndTime_Close(info, 1, 0);           //修改该账号的登陆状态到数据库

                    dataGridView1.Invoke((MethodInvoker)(() =>          //更新服务器的显示信息
                    {
                        ShowClientInfo();
                    }));
                    return "12";
                case "22":                                              //代表给客户端发送ip port对应的用户信息
                    //代表给客户端发送ip port对应的用户信息  
                    temp = new List<object>();
                    temp.Add(info[1].ToString());
                    temp.Add(info[2].ToString());
                    return "22";
                default:
                    return "0";                                         //代表不存在
            }
        }

        //写入在服务器显示的信息(服务端收到的客户端的信息)
        private void ShowInfoOnView(List<Object> info,string i)
        {
            switch (info[0].ToString())
            {
                case "1":
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[0].ToString()); //显示字符串  
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[1].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[2].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[3].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[4].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[5].ToString());
                    break;
                case "3":
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[0].ToString());  //显示字符串  
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[1].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[2].ToString());
                    break;
                case "7":
                    OperateAboutUIControl_RichTextBox(richTextBox1, info[0].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1, info[1].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1, info[2].ToString());
                    break;
                case "12":
                    OperateAboutUIControl_RichTextBox(richTextBox1,  info[0].ToString());
                    break;
                case "22":                                              //代表给客户端发送ip port对应的用户信息
                    OperateAboutUIControl_RichTextBox(richTextBox1, info[0].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1, info[1].ToString());
                    OperateAboutUIControl_RichTextBox(richTextBox1, info[2].ToString());
                    break;

            }
        }   

        //确定注册或者登陆成功后  需要发给客户端的信息
        private List<Object> SendAboutLoOrRe(List<Object> infolist)
        {
            switch (infolist[0].ToString())
            {
                case "2":                               //注册成功
                    return infolist;
                case "4":                               //登陆成功
                    SaveClientInfoToList(infolist);     //将所有的id信息放到infolist
                    return infolist;
                case "7":                               //登陆后 每隔一段时间更新好友列表（刷新好友列表）
                    SaveClientInfoToList(infolist);     //将所有的id信息放到infolist
                    return infolist;
                case "12":                              //退出成功
                    return infolist;
                case "22":                              //代表给客户端发送ip port对应的用户信息
                    infolist = GetUserInfo(temp, infolist);
                    return infolist;
                case "23":                              //当客户账号已经存在就返回23 注册失败
                    return infolist;
                default:                                //其他情况 一般不会出现
                    return infolist;
            }
        }

        //修改该账号的登陆状态到数据库 客户端信息 登陆状态     loginOrclose 1 代表登陆 0代表下线
        private void UpdateLoginStateAndTime_Close(List<Object> info,int isloginState,int loginOrclose)
        {
            string updateStr;
            if (loginOrclose == 1)
            {
                updateStr = String.Format("update IDInfo set lastlogindate ='{0}',islogin={1} where ip='{2}' and port = {3} and pwd='{4}' ",
                       DateTime.Now.ToString(),
                       isloginState,
                       info[1].ToString(),
                       Convert.ToInt32(info[2].ToString()),
                       info[3], ToString());
            }
            else 
            {
                if (info[1] == null)                                                      //如果是在主窗体之外退出的 则info只有一条数据 其他的都为空
                    return;
                updateStr = String.Format("update IDInfo set islogin={0} where ip='{1}' and port = {2}",
                    0,
                    info[1].ToString(),
                    Convert.ToInt32(info[2].ToString())
                    );
            }
            DBDeal dbDeal = new DBDeal();
            string tips = (loginOrclose == 1) ? "上线" : "下线";                    //  提示语句
            int i = dbDeal.SqlAction(updateStr);
            if (i > 0)
            {
                OperateAboutUIControl_RichTextBox(richTextBox1, info[1].ToString() + tips);
            }
        }

        //将所有的id信息放到infolist 返回到客户端的信息
        private List<Object> SaveClientInfoToList(List<Object> infolist)
        {
            string selectStr = "select name,ip,port,islogin from IDInfo";
            DBDeal dbDeal = new DBDeal();
            SqlDataReader sdr = dbDeal.SqlGetThreeInfo(selectStr);
            while(sdr.Read())
            {
                infolist.Add(sdr[0].ToString());//name
                infolist.Add(sdr[1].ToString());//ip
                infolist.Add(sdr[2].ToString());//port
                infolist.Add(sdr[3].ToString());//islogin
            }
            dbDeal.ShutDown();
            return infolist;
        }

        //关闭服务
        private void CloseServer()
        {
            sign = false;
            if(server != null)
                server.Close();
            richTextBox1.AppendText("\n 已关闭服务器！！！");
            
            
        }

        //操作所有的关于UI 的RichTextBox控件的信息
        private void OperateAboutUIControl_RichTextBox(RichTextBox richTextBox,string str)
        {
            richTextBox.Invoke(
                   (MethodInvoker)
                   (() =>
                   {
                       richTextBox.AppendText("\n"+str);
                       richTextBox.ScrollToCaret();
                   }));
        }

        //根据ip port获取信息 （22）
        private List<Object> GetUserInfo(List<Object> temp, List<Object> infolist)
        {
            string selectStr = String.Format("select * from IDInfo where ip='{0}' and port='{1}'", 
                temp[0].ToString(),
                temp[1].ToString());
            DBDeal dbDeal = new DBDeal();
            SqlDataReader sdr = dbDeal.SqlGetThreeInfo(selectStr);
            sdr.Read();
            infolist.Add(sdr[0].ToString());//id 
            infolist.Add(sdr[1].ToString());//ip
            infolist.Add(sdr[2].ToString());//port
            //infolist.Add(sdr[3].ToString());//pwd
            infolist.Add(sdr[4].ToString());//name
            infolist.Add(sdr[5].ToString());//sex
            infolist.Add(sdr[6].ToString());//lastlogindate
            infolist.Add(sdr[7].ToString());//islogin
            infolist.Add(sdr[8].ToString());//other
            dbDeal.ShutDown();
            return infolist;
        }

        //注册的时候 对比信息 是否已有账号 已经存在就返回true  不存在返回false
        private bool IsExistID(List<Object> info)
        {
            string selectstr = String.Format("select * from IDInfo where ip='{0}' and port = {1}",
                    info[1].ToString(),
                    Convert.ToInt32(info[2].ToString()));
            DBDeal dbDeal2 = new DBDeal();

            return dbDeal2.SqlSelectHas(selectstr);                //已经存在账号则返回客户端 
        }

        #endregion

        #region 显示客户端信息 DataGridView

        private void ShowClientInfo()
        {
            DBDeal dbDeal = new DBDeal();
            string selectstr = "select * from IDInfo";
            dbDeal.SqlFullDataGridView(dataGridView1,selectstr);
        }

        #endregion

        #region 注销客户信息

        void ClientContextMenuStrip()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ContextMenuStrip cms = new ContextMenuStrip ();
            ToolStripMenuItem cms1 = new ToolStripMenuItem("删除");
            cms.Items.Add(cms1);
            cms1.Click += cms1_Click;
            dataGridView1.ContextMenuStrip = cms;
        }

        void cms1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            DeleteClientInfo();
            //刷新客户列表
            ShowClientInfo();
        }

        void DeleteClientInfo()
        {
            string ip = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            int port = Convert.ToInt32(dataGridView1.CurrentRow.Cells[2].Value.ToString());
            string pwd = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            dataGridView1.CurrentRow.Cells[0].Value.ToString();
            string deleteStr = String.Format("delete from IDInfo where ip='{0}' and port = {1} and pwd='{2}' ",
                       ip,
                       port,
                       pwd);
            DBDeal dbDeal = new DBDeal();
            int i = dbDeal.SqlAction(deleteStr);
            if (i > 0)
            {
                OperateAboutUIControl_RichTextBox(richTextBox1,
                    ip + "\n" +port +"\n"+ pwd + "\n已删除！！！");
            }
        }

        #endregion
    }
}
