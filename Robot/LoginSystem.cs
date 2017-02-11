using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;
namespace Robot
{

    public class LoginSystem : BaseSystem
    {
        public LoginSystem(Actor pActor, Session pSession) : base(pActor,pSession)
        {
        }
        public void ProcessNetData(PackIn pack)
        {

            byte id = pack.ReadByte();
            switch (id)
            {
                case PacketTypes.LoginPacketTypes.sQueryRole:
                    {
                        DoQueryRole(pack);

                        break;
                    }
                default:
                    {
                        Console.WriteLine("登录系统未知协议号:" + id);
                        break;
                    }
            }
        }

        private void DoQueryRole(PackIn pack)
        {
            uint nAccountId = pack.ReadUInt32();
            byte nCount = pack.ReadByte();
            if (nCount >= 0 && nCount < 100)
            {
               
                for (int i = 0; i < /*nCount*/ 1/*取第一个角色*/; i++)
                {
                    TSimplePlayerInfo info = new TSimplePlayerInfo();
                    info.PackToStruct(pack);
                    info.nAccountId = nAccountId;
                    m_pActor.SimplePlayerInfo = info;
                    
                    Console.WriteLine("登录游戏成功:" + m_pSession.Account+"  选择进入游戏:"+info.szName);
                }
                this.SendEnterGameScene(nAccountId, m_pActor.SimplePlayerInfo.nId);
            }
           
        }
        private void SendEnterGameScene(uint nAccountId, uint nRoleId)
        {
            PacketOut pack = new PacketOut();
            pack.WriteCmd(PacketTypes.LOGIC, PacketTypes.LogicPacketTypes.cLoginGame);
            pack.WriteUInt32(nAccountId);
            pack.WriteUInt32(nRoleId);
            pack.WriteUInt32(0); //出生点id
            m_pSession.Send(pack.GetBuffer());
        }

    }
}
