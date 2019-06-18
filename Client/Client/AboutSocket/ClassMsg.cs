using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.AboutSocket
{

    /// <summary>
    /// 消息命令
    /// </summary>
    public enum MsgCommand
    {
        None,           //不存在0 //
        Registering,    //注册1   //
        Registered,     //注册结束2//
        Logining,       //登陆3   //
        Logined,        //登陆完毕   上线4    //
        SendToOne,      //发给一个人5
        SendToAll,      //发给所有用户6
        UserList,       //用户列表7 //
        UpdateState,    //更新用户状态8
        VideoOpen,      //打开视频9
        Videoing,       //正在视屏10
        VideoClone,     //结束视屏11
        Close,          //下线12  //
        GetUserInfo,     //获取用户信息 //22
        UserIsExist     //用户已存在23
    }
    /// <summary>
    /// 发送类型
    /// </summary>
    public enum SendKind
    {
        SendNone,       //无类型13
        SendMommand,    //发送命令14
        SendMsg,        //发送消息15
        SendFile        //发送文件16
    }
    /// <summary>
    /// 发送信息的状态
    /// </summary>
    public enum SendState
    {
        None,           //无状态17
        Single,         //单消息 文件18
        Start,          //发送开始生成文件19
        Sending,        //正在发送 写入文件20
        End             //发送结束21
    }
}




///// <summary>
///// 用枚举类型的元素值指定消息发送的命令 消息类型 消息发送状态
///// [Serializable] 并序列化   
///// 序列化后  序列化引擎跟踪所有的已经序列化的引用对象 保证不被多次序列化
///// </summary>
//[Serializable]
//public class ClassMsg
//{
//    public string SID = "";         //发送端
//    public string SIP = "";
//    public string SPort = "";
//    public string RID = "";         //接收端
//    public string RIP = "";
//    public string RPort = "";

//    public SendKind sendkind = SendKind.SendNone;   //默认发送状态为 无类型
//    public MsgCommand msgcommand = MsgCommand.None; //默认消息命令 状态
//    public SendState sendstate = SendState.None;    //默认发送状态

//    public string msgID = "";  //消息ID    这两个什么鬼
//    public byte[] Data;
//}
///// <summary>
///// 注册用户信息
///// </summary>
//[Serializable]
//public class RegisterMsg
//{
//    public string UserName;
//    public string PassWord;
//}