using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDPTest;

namespace Client
{
    public partial class TinyFileTrans : Form
    {

        public string Ip { get; set; }          //本机ip
        public int Port { get; set; }        //本机port
        public string IpO { get; set; }          //对方ip
        public int PortO { get; set; }        //对方port
        int filePort = 45550;

        public int FilePort
        {
            get { return filePort; }
            set { filePort = value; }
        }
                
        public void TransInfo(string ip, int port, string ipO, int portO)
        {
            Ip = ip;
            Port = Convert.ToInt32(port);
            IpO = ipO;
            PortO = Convert.ToInt32(portO);
            this.Text = " 和IP： " + ipO + " Port： " + portO + " 文件互送";
        }

        public TinyFileTrans()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendFile();
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReceiveFile();
            button2.Enabled = false;
        }

        //发送文件
        private void SendFile()
        {
            OpenFileDialog opfd = new OpenFileDialog();
            if (opfd.ShowDialog() == DialogResult.OK)
            {
                //获取文件地址
                string msg = Path.GetFullPath(opfd.FileName);
                //打开文件 得到文件信息
                FileStream fileS = File.Open(msg, FileMode.Open, FileAccess.Read);
                //用于存文件信息
                byte[] filedata = new byte[fileS.Length];
                //流中的信息放到 byte[]
                fileS.Read(filedata, 0, filedata.Length);

                fileS.Flush();
                fileS.Close();

                TcpFileTrans tft = new TcpFileTrans(Ip, FilePort, IpO, FilePort);

                //开始发送文件
                tft.Infolist = new List<object>();
                tft.Infolist.Add("16");
                tft.Infolist.Add(Ip);
                tft.Infolist.Add(Port);
                tft.Infolist.Add(DateTime.Now.ToString());
                tft.Infolist.Add(filedata);

                if (tft.YM_SendMessage() == false)
                {
                    MessageBox.Show("端口已经被使用", "提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

            }
        }

        //接收文件
        private void ReceiveFile()
        {
            TcpFileTrans tft = new TcpFileTrans(Ip, FilePort, IpO, FilePort);
            //接收文件
            int switch_Msg = tft.YM_ReceiveMessageAndSendOk(richTextBox1);
            switch (switch_Msg)
            {
                case 0:
                    break;
                default:
                    MessageBox.Show("对方并没有发送文件",
                        "提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
            }

            //发送确认信息
            //tft.YM_BeHandReceiveMessage(richTextBox1);
        }
    }
}
