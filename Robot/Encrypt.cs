using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**
 * 数据加密解密处理类
 * @author 后天
 * 
 */	
namespace Robot
{
    public class Encrypt
    {
        private  uint SelfSalt ;
        private  uint TargetSalt = 0;
        private  uint Key = 0;
        private  byte[] KeyBuff = new byte[4];
        public Encrypt()
        {
            SelfSalt = MakeSalt();
         
        }
        private  uint MakeSalt()
        {
            uint d = (uint)System.Environment.TickCount;
            Random rd = new Random();
            uint dd = (uint)rd.Next() * d;
            return dd;
        }

        public  uint GetSelfSalt()
        {
            return SelfSalt;
        }

        public  uint GetTargetSalt()
        {
            return TargetSalt;
        }
        public  uint GetCRC16(byte[] pBytes,uint pLen = 0)
        {
            return CRC16.Update(pBytes,0,pLen);
        }
        public  void SetTargetSalt(uint pSalt)
        {
            TargetSalt = pSalt;
            MakeKey();
        }
        public  uint getCheckKey()
        {
            GameCommon.PacketOut pack = new GameCommon.PacketOut();
            pack.WriteUInt32(Key);
            byte[] data = pack.GetBuffer();
            uint ck = CRC16.Update(data);
            return ck;
        }
        private  void MakeKey()
        {
            Key = (SelfSalt ^ TargetSalt) + SelfSalt + 8654;
            for (int i = 0; i < 4; i++)
            {
                KeyBuff[i] =(byte)( (Key & (0xFF << (i << 3))) >> (i << 3)); 
            }
        }

        public  uint Encode(byte[] pInBuff,uint pOffset = 0,uint pLength = 0)
        {
            if (pOffset >= pInBuff.Length)
            {
                return 0;
            }
            uint end = pLength > 0 ? pOffset + pLength : (uint)pInBuff.Length;
            if (end > pInBuff.Length)
            {
                end = (uint)pInBuff.Length;
            }
            for (uint i = pOffset; i < end; i++)
            {
                pInBuff[i] ^= KeyBuff[i % 4];
            }
            return end - pOffset;
        }
        public uint Decode(byte[] pInBuff, uint pOffset = 0, uint pLength = 0)
        {
            //当前的加密算法和解密算法是一样的,反向操作
            return Encode(pInBuff, pOffset, pLength);
        }
    }
}
