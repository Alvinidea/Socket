using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.AboutSocket
{
    /* 
     数据传输的格式为 
     * 标号        （暂时留着）       
     * ip
     * port
     * 发送日期
     * 发送的信息    
     */
    class UdpSkt
    {
        #region 基础信息

        public Socket udpChat = null;
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
        public UdpSkt(string ipstr, int portstr)
        {
            this.IpYou = ipstr;
            this.PortYou = portstr;
            GetYouUdpPoint();
            GetUdpSocket();
        }
        //用于接收消息
        public UdpSkt(string ipstr, int portstr, string ipstrY, int portstrY)
        {
            this.IpYou = ipstrY;
            this.PortYou = portstrY;
            this.IpMe = ipstr;
            this.PortMe = portstr;
            GetYouUdpPoint();
            GetMeUdpPoint();
            GetUdpSocket();
        }

        #endregion

        #region 获取IPEndPoint
        

        public IPEndPoint GetYouUdpPoint()
        {
            LocalYou = IPAddress.Parse(IpYou);
            IpPortYou = new IPEndPoint(LocalYou, PortYou);
            return IpPortYou;
        }

        public IPEndPoint GetMeUdpPoint()
        {
            LocalMe = IPAddress.Parse(IpMe);
            IpPortMe = new IPEndPoint(LocalMe, PortMe);
            return IpPortMe;
        }

        #endregion

        #region 获取UDP Socket

        public Socket GetUdpSocket()
        {
            udpChat = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            return udpChat;
        }

        #endregion

        #region 发送数据

        public void YM_SendMessage(List<Object> infolist)
        {
            //获取socket
            Socket sendSocket = GetUdpSocket();
            //获取对方 ip port
            EndPoint youPoint = GetYouUdpPoint() as EndPoint;
            //将数据转换为byte[]
            byte[] dataSe = ClassSerializers.Serializebinary(infolist).ToArray();
            //发送数据
            sendSocket.SendTo(dataSe, youPoint);
            //关闭Socket
            sendSocket.Close();
        }

        #endregion

        #region 接收数据

        public void YM_ReceiveMessage(RichTextBox richTextBox)
        {
            //获取socket
            Socket receiveSocket = GetUdpSocket();
            //获取本地 ip port
            IPEndPoint mePoint = GetMeUdpPoint();
            //绑定本地ip port
            receiveSocket.Bind(mePoint);
            //获取对方 ip port
            EndPoint youPoint = GetYouUdpPoint() as EndPoint;          
            //用于存放信息
            byte[] dataRe = new byte[1024];
            while(true)
            {
                try
                {
                    //在这儿的时候有一个异常(一个封锁操作被对 WSACancelBlockingCall 的调用中断。)
                    //try catch 获取异常但是不管它 直接返回
                    int datalength = receiveSocket.ReceiveFrom(dataRe, ref youPoint);

                    List<Object> listinfo = (ClassSerializers.DeSerializebinary(new MemoryStream(dataRe))) as List<Object>;
                    richTextBox.Invoke(
                        (MethodInvoker)
                        (() =>
                        {
                            richTextBox.AppendText(InfoTransToString(listinfo));
                            richTextBox.ScrollToCaret();
                        }));
                }
                catch 
                {
                    break;
                    return;
                }

            }
        }
        //用于将 List<Object> 中信息转换为string
        private string InfoTransToString(List<Object> listinfo)
        {
            return listinfo[1].ToString() + "  " +//IP
                listinfo[2].ToString() + "  " +//Port
                listinfo[3].ToString() + "\n" +//Date
                listinfo[4].ToString() + "\n";//infomation
        }

        #endregion

    }
}
