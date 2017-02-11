using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    //系统id
    public class PacketTypes
    {
        public const byte LOGIN = 255;

        //登录系统协议号
        public class LoginPacketTypes
        {
            public const byte LOGIN = 1;//校验帐号是否可以登录
        }
    }


}
