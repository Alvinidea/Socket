using Client.AboutSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ChatForm : Form
    {
        public string Ip { get; set; }          //本机ip
        public int Port { get; set; }        //本机port
        public string IpO { get; set; }          //对方ip
        public int PortO { get; set; }        //对方port
        UdpSkt udpskt = null;
        //给本机和对方Ip Port      还有对应标签赋值 
        public void TransInfo(string ip, string port,string ipO,string portO)
        {
            Ip = ip;
            Port = Convert.ToInt32(port);
            IpO = ipO;
            PortO = Convert.ToInt32(portO);
            this.Text =" 与IP： "+ ipO + " Port： " + portO + " 的聊天室";
        }   

        //----------------------------------
        ContextMenuStrip cms;
        //----------------------------------
        public ChatForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load+=ChatForm_Load;
            this.FormClosed += ChatForm_FormClosed;
            textBoxForm.KeyDown += textBoxForm_KeyDown;
            textBoxForm.KeyUp += textBoxForm_KeyUp;
            sendMessage.Click += sendMessage_Click;
            RichTextBoxDeal();
            sendFile.Click += sendFile_Click;
            
        }

        //收消息 登陆后就开启端口监听
        private void ChatForm_Load(object sender, EventArgs e)
        {
            List<Object> info = this.Tag as List<object>; //从主界面传来的信息 IP Port
            TransInfo(info[0].ToString(), info[1].ToString(), info[2].ToString(), info[3].ToString());
            Thread thread = new Thread(new ThreadStart(Receive));
            thread.IsBackground = true;
            thread.Start();
        }

        //关闭窗体之前释放资源
        void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //想要释放资源   解除绑定    
            if (udpskt.udpChat.IsBound)
            {
                //关闭的时候让 取消绑定   不然端口会一直绑定  下次进入聊天界面后就不能绑定端口
                udpskt.udpChat.Shutdown(SocketShutdown.Both);
                udpskt.udpChat.Dispose();
            }
        }

        #region 事件处理

        #region 接收消息
        
        #endregion

        #region 发送信息

        //点击发送
        void sendMessage_Click(object sender, EventArgs e)
        {
            string msg = MessageDeal();
            if (msg == "")
                return;
            MessageSendBeyond(msg);
            textBoxForm.Text = "";
        }
        //回车发送
        void textBoxForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendMessage_Click(sender, e);
            }
            if (e.KeyData == (Keys.Enter|Keys.Shift))
            {
                textBoxForm.Text += "\n";
            }
        }
        //清除回车
        void textBoxForm_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode ==  Keys.Enter)
                textBoxForm.Clear();
            
        }

        #endregion

        #endregion

        #region 事件中的处理函数

        //线程中收消息
        private void Receive()
        {
            udpskt = new UdpSkt(Ip, Port, IpO, PortO);
            udpskt.YM_ReceiveMessage(richTextBoxForm);
        }

        //信息在本机的发送（显示在richTextBox中）
        private string MessageDeal()
        {
            if (textBoxForm.Text.Trim() == "")  
            {
                textBoxForm.Text = "";
                return "";
            }
            string message ="自己  :" + Ip + "  " + Port + "  " + DateTime.Now.ToString() + "\n";
            message += textBoxForm.Text+"\n";
            richTextBoxForm.AppendText( message);
            richTextBoxForm.ScrollToCaret();

            //更改发送字体的颜色    没有实现
            //FormShow.RichTextBoxColorChange(richTextBoxForm,message);

            return message;
        }
        //信息发送到远端（显示）
        private void MessageSendBeyond(string message)
        {
            List<Object> listinfo = new List<object>();
            listinfo.Add("1");          //无意义消息 以后可能有用
            listinfo.Add(Ip);           //自己的ip
            listinfo.Add(Port);         //自己绑定的port
            listinfo.Add(DateTime.Now.ToString());
            listinfo.Add(textBoxForm.Text);
            UdpSkt udpskt = new UdpSkt(IpO, PortO);
            udpskt.YM_SendMessage(listinfo);
        }

        #endregion

        #region RichTextBox 处理

        void RichTextBoxDeal()
        {
             cms = new ContextMenuStrip ();
            ToolStripMenuItem cms1 = new ToolStripMenuItem("剪切");
            ToolStripMenuItem cms2 = new ToolStripMenuItem("复制");
            ToolStripMenuItem cms3 = new ToolStripMenuItem("粘贴");
            ToolStripMenuItem cms4 = new ToolStripMenuItem("全选");
            cms.Items.Add(cms1);
            cms.Items.Add(cms2);
            cms.Items.Add(cms3);
            cms.Items.Add(cms4);
            richTextBoxForm.ContextMenuStrip = cms;

            cms1.Click += cms1_Click;
            cms2.Click += cms2_Click;
            cms3.Click += cms3_Click;
            cms4.Click += cms4_Click;
        }

        void cms1_Click(object sender, EventArgs e)
        {
            richTextBoxForm.Cut();
        }

        private void cms2_Click(object sender, EventArgs e)
        {
            richTextBoxForm.Copy();
        }

        private void cms3_Click(object sender, EventArgs e)
        {
            richTextBoxForm.Paste();
        }

        private void cms4_Click(object sender, EventArgs e)
        {
            richTextBoxForm.SelectAll();
        }



        #endregion

        void sendFile_Click(object sender, EventArgs e)
        {
            TinyFileTrans tft = new TinyFileTrans();
            tft.TransInfo(Ip, Port, IpO, PortO);
            tft.Show(this);
        }
    }
}
