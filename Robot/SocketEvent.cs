using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCommon
{
     
    //服务器端的套接字事件
    public struct SocketEvent
    {
        public enum SOCKETEVENT_TYPE
        {
            ONACCEPT = 1,
            ONSEND,
            ONCLOSE,
            ONRECVDATA,
            ONSHUTDOWN,
        }
        public IntPtr connId;
        public SOCKETEVENT_TYPE type;
        public byte[] data;
    }

    public struct ClientSocketEvent
    {
         public enum CLIENTSOCKETEVENT_TYPE
        {
            NORMAL = 0,
            ONPREPARECONNECT,   //监听
            ONCONNECT,      //连接
            ONSEND,         //发送数据
            ONRECEIVE,      //返回数据
            ONCLOSE,        //被断开
            ONRECONNECT, //重连事件
        }
         public IntPtr connId;
         public CLIENTSOCKETEVENT_TYPE type;
         public byte[] data;
  

    }
}
