using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Client.AboutSocket
{
    public class ClassSerializers
    {
        public static MemoryStream Serializebinary(object request)//对象序列化为二进制流
        {            
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer = new
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream menStream = new MemoryStream();    //创建一个内存流存储区
            serializer.Serialize(menStream, request);       //将对象序列化为二进制流
            return menStream;
        }
        public static object DeSerializebinary(MemoryStream memStream)//二进制流反序列化为对象
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter deserializer = new
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            object newobj = deserializer.Deserialize(memStream);    //创建一个内存流存储区
            memStream.Close();                                     //关闭内存流 并释放
            return newobj;
        }
    }
}
