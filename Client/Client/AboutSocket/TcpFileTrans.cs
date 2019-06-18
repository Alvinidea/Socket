using Client.AboutSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPTest
{
    class TcpFileTrans
    {
        #region 基础信息

        Socket TcpFileSocket = null;
        public string IpYou { get; set; }       //对面客户端ip 默认本机环回地址
        public int PortYou { get; set; }        //对面客户端port  默认45556
        public IPAddress LocalYou { get; set; }     //对面客户端ip
        public IPEndPoint IpPortYou { get; set; }   //对面客户端port

        public string IpMe { get; set; }        //本机客户端ip 默认本机环回地址
        public int PortMe { get; set; }         //本机客户端port  默认45556
        public IPAddress LocalMe { get; set; }     //对面客户端ip
        public IPEndPoint IpPortMe { get; set; }   //对面客户端port

        #endregion      

        #region 构造方法
        //用于发送消息
        public TcpFileTrans(string ipstr, int portstr)
        {
            this.IpYou = ipstr;
            this.PortYou = portstr;
            GetYouTcpPoint();
            GetTcpSocket();
        }
        //用于接收消息
        public TcpFileTrans(string ipstr, int portstr, string ipstrY, int portstrY)
        {
            this.IpYou = ipstrY;
            this.PortYou = portstrY;
            this.IpMe = ipstr;
            this.PortMe = portstr;
            GetYouTcpPoint();
            GetMeTcpPoint();
            GetTcpSocket();
        }

        #endregion

        #region 获取IPEndPoint


        public IPEndPoint GetYouTcpPoint()
        {
            LocalYou = IPAddress.Parse(IpYou);
            IpPortYou = new IPEndPoint(LocalYou, PortYou);
            return IpPortYou;
        }

        public IPEndPoint GetMeTcpPoint()
        {
            LocalMe = IPAddress.Parse(IpMe);
            IpPortMe = new IPEndPoint(LocalMe, PortMe);
            return IpPortMe;
        }

        #endregion

        #region 获取Tcp Socket
        // 获取Tcp Socket
        public Socket GetTcpSocket()
        {
            TcpFileSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            return TcpFileSocket;
        }

        #endregion

        //用于与类之外的信息交流
        public List<Object> Infolist { get; set; }

        #region 发送数据 (主动发文件的当服务端)

        //发送消息
        public bool YM_SendMessage()
        {
            try
            {
                //获取socket
                TcpFileSocket = GetTcpSocket();
                //获取自己 ip port
                IPEndPoint mePoint = GetMeTcpPoint();
                //绑定端口  如果已经绑定那么 try catch
                TcpFileSocket.Bind(mePoint);
                //开始监听
                TcpFileSocket.Listen(10);

                for (int o = 0; o <= 0; o++) //  1  打开后只可以连入两次
                {
                    Thread serverthread = new Thread(new ThreadStart(TheServerThread));
                    serverthread.IsBackground = true;
                    serverthread.Start();
                }
                return true;
            }
            catch
            {
                for (int o = 0; o <= 0; o++) //  1  打开后只可以连入两次
                {
                    Thread serverthread = new Thread(new ThreadStart(TheServerThread));
                    serverthread.IsBackground = true;
                    serverthread.Start();
                }
                return true;
            }
        }
        //发送消息需要调用的函数 
        private void TheServerThread()
        {
            try
            {
                //接受连接
                Socket sonS = TcpFileSocket.Accept();
                //将数据转换为byte[]
                byte[] dataSe = ClassSerializers.Serializebinary(Infolist).ToArray();
                //发送数据(文件)
                sonS.Send(dataSe, dataSe.Length, SocketFlags.None);
                //byte[]
                byte[] dataSe2 = new byte[2048];
                //接收信息 返回对方是否收到的信息
                sonS.Receive(dataSe2, dataSe2.Length, SocketFlags.None);

                List<Object> msg = ClassSerializers.DeSerializebinary(new MemoryStream(dataSe2)) as List<Object>;

                if (msg[4].ToString() == "ok")
                {
                    MessageBox.Show("--------文件传输完毕-------",
                        "ok",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Question);
                }
                else
                {
                    MessageBox.Show("--------文件传输失败-------",
                        "GG",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

            }
            catch
            {
                return;
            }
        }

        #endregion

        #region 主动接收数据（文件）

        /// <summary>
        /// 主动接收数据（文件）   
        /// 0 代表 无错误 
        /// 1 代表 SocketException
        /// 2 未知错误
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <returns></returns>
        public int YM_ReceiveMessageAndSendOk(RichTextBox richTextBox)
        {
            try
            {
                //获取socket
                Socket receiveSocket = GetTcpSocket();
                //获取本地 ip port
                IPEndPoint youPoint = GetYouTcpPoint();
                //连接对方
                receiveSocket.Connect(youPoint);
                //用于存放信息
                byte[] dataRe = new byte[10240];

                if (receiveSocket.Connected)
                {
                    //接收消息
                    receiveSocket.Receive(dataRe, dataRe.Length, SocketFlags.None);
                    //提取信息
                    List<Object> listinfo = (ClassSerializers.DeSerializebinary(new MemoryStream(dataRe))) as List<Object>;
                    //显示信息
                    richTextBox.Invoke(
                        (MethodInvoker)
                        (() =>
                        {
                            richTextBox.AppendText(InfoTransToString(listinfo));
                            richTextBox.ScrollToCaret();
                            //存文件
                            SaveFile(listinfo);
                        }));
                    List<object> infolist = new List<object>();
                    infolist.Add("16");
                    infolist.Add(IpMe);
                    infolist.Add(PortMe);
                    infolist.Add(DateTime.Now.ToString());
                    infolist.Add("ok");
                    //确认消息
                    byte[] dataRe2 = ClassSerializers.Serializebinary(infolist).ToArray();
                    //发送确认消息
                    receiveSocket.Send(dataRe2, dataRe2.Length, SocketFlags.None);
                }
                //receiveSocket.Close();
                return 0;
            }
            catch (Exception e)
            {
                if (e is SocketException)
                    return 1;
                else 
                {
                    return 2;
                }
            }
        }

        void SaveFile(List<Object> listinfo)
        {
            byte[] file = listinfo[4] as byte[];
            //文件放在D:盘
            FileStream fileS = new FileStream (@"D:\File"+ new Random().Next(100)+".txt",FileMode.Create,FileAccess.Write);

            fileS.Write(file, 0, file.Length);

            fileS.Flush();
            fileS.Close();
        }

        //用于将 List<Object> 中信息转换为string
        private string InfoTransToString(List<Object> listinfo)
        {
            return listinfo[1].ToString() + "  " +
                listinfo[2].ToString() + "  " +
                listinfo[3].ToString() + "\n" +
                "文件接收完毕" + "\n";                        // 将文件接收信息 显示在界面
        }



        /*
         * //发送确认信息
        public void YM_BeHandReceiveMessage(RichTextBox richTextBox)
        {
            try
            {
                //获取socket
                Socket receiveSocket = GetTcpSocket();
                //获取本地 ip port
                IPEndPoint youPoint = GetYouTcpPoint();
                //连接对方
                receiveSocket.Connect(youPoint);

                if (receiveSocket.Connected)
                {
                    List<object>  infolist = new List<object>();
                    infolist.Add("16");
                    infolist.Add(IpMe);
                    infolist.Add(PortMe);
                    infolist.Add(DateTime.Now.ToString());
                    infolist.Add("ok");
                    //确认消息
                    byte[] dataRe2 = ClassSerializers.Serializebinary(infolist).ToArray();
                    //发送确认消息
                    receiveSocket.Send(dataRe2, dataRe2.Length, SocketFlags.None);
                }
                receiveSocket.Close();
            }
            catch 
            {
                return;
            }
        }
        */

        #endregion

    }

}
