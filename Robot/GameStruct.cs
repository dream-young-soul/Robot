using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;
namespace Robot
{
    public struct TSimplePlayerInfo
    {
        public uint nAccountId; //帐号id
        public string szName;
        public byte bFaceId;    //头像
        public byte bLevel;      //等级
        public byte bJob;       	//职业
        public uint nId;            //角色id
        public byte bSex;            //性别
        public string szGuildName;    //行会名  
        public byte bCircle;     //转生
        public int bApoteoSize; //封神    
        public byte bState;     //0 角色被删 1 被封号 2 正常
        public void PackToStruct(PackIn pack)
        {
            nId = pack.ReadUInt32();
            szName = pack.ReadString();
            bFaceId = pack.ReadByte();
            bSex = pack.ReadByte();
            bLevel = pack.ReadByte();
            bJob = pack.ReadByte();
            pack.ReadByte(); //原本的阵营位
            bCircle = pack.ReadByte();
            bApoteoSize = pack.ReadByte();
            szGuildName = pack.ReadString();
            bState = pack.ReadByte();
           
       
        }
    }
}
