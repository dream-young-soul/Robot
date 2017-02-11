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
        public const byte LOGIC = 0;
        public const byte LOGIN = 255;

        //登录系统协议号
        public class LoginPacketTypes
        {
            public const byte cLogin = 1;//校验帐号是否可以登录

            public const byte sQueryRole = 2;   //查询角色
        }

        //逻辑系统协议号
        public class LogicPacketTypes
        {
            public const byte cLoginGame = 1;   //登录游戏
        }
    }


}
