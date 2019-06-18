using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.AboutSocket
{
    class TcpSkt
    {
        string ip = "127.0.0.1";        //默认服务端ip      
        int port = 45555;               //服务端port
        IPAddress local = null;         //服务端ip
        IPEndPoint ipPort = null;       //服务端port
        Socket client = null;           
        public TcpSkt() 
        {


            this.ip = GetMyselfIP(); //本机测试用

            //当server端的ip一直变化时候   在配置文件里更改服务端ip
            //this.ip = ConfigurationManager.AppSettings["serverIp"].ToString();
            GetTcpPoint();
            GetTcpSocket();
        }

        public TcpSkt(string ipstr,int portstr)
        {
            this.ip = ipstr;
            this.port = portstr;
            GetTcpPoint();
            GetTcpSocket();
        }

        public IPEndPoint GetTcpPoint()
        {
            local = IPAddress.Parse(ip);
            ipPort = new IPEndPoint(local, port);
            return ipPort;
        }

        public Socket GetTcpSocket()
        {
            client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            return client;
        }

        public List<Object> LR_ActionSend_Receive(List<Object> infolist)
        {
            try
            {
                Byte[] dataReceive = new Byte[1024];        //存放接收数据
                List<Object> dataRece = null;               //反序列化后的数据
                client.Connect(GetTcpPoint());              //连接服务器
                if (client.Connected)
                {
                    byte[] data = ClassSerializers.Serializebinary(infolist).ToArray();
                    client.Send(data, data.Length, 0);
                    client.Receive(dataReceive, dataReceive.Length, 0);   //接收数据    //在这等着呢 -_-
                    dataRece = ClassSerializers.DeSerializebinary(new MemoryStream(dataReceive)) as List<Object>;
                }
                client.Close();
                return dataRece;
            }
            catch
            {
                return null;
            }
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
    }
}
