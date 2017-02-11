using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;
namespace Robot
{
   public class GamePackEx
    {
      public static int DEFAULT_TAG = 0xCCEE;//约定的信息头
      private Queue<byte[]> m_ListData;
      private MemoryStream m_stream;
      public GamePackEx()
      {
          m_stream = new MemoryStream();
          m_ListData = new Queue<byte[]>();

      }
       public void ProcessNetData(byte[] data)
       {
           byte[] dedata = new byte[data.Length];
           Buffer.BlockCopy(data, 0, dedata, 0, data.Length);
           m_stream.Write(dedata, 0, dedata.Length);
           PackIn packin = new PackIn(m_stream.GetBuffer());
           int nCurPos = 0; //记录当前流位置
           while (true)
           {
               ushort tag = packin.ReadUInt16();
               nCurPos += sizeof(ushort);
               if (tag != DEFAULT_TAG)
               {
                   continue;
               }
               int nLen = packin.ReadUInt16();
               nCurPos += sizeof(ushort);
               //    //预留4字节
               packin.ReadUInt32();
               nCurPos += sizeof(uint);
               //    //读取消息消耗时间
               packin.ReadInt32();
               nCurPos += sizeof(int);
               packin.ReadInt32();
               nCurPos += sizeof(int); 
               if (nLen > m_stream.Length  - nCurPos) //封包不是完整的
               {
                   if (m_stream.Length == dedata.Length) //非法封包
                   {
                       m_stream.SetLength(0);
                       return;
                   }
                   break;
               }
               nCurPos += nLen;
               byte[] reData = packin.ReadBuff(nLen);
               if (reData == null || reData.Length == 0) break;
               m_ListData.Enqueue(reData);
               if (nCurPos == m_stream.Length) break;
           }
           int rema_Len = (int)m_stream.Length - nCurPos;
           if (rema_Len > 0)
           {
               dedata = new byte[rema_Len];
               Buffer.BlockCopy(m_stream.GetBuffer(), nCurPos, dedata, 0, rema_Len);
           }
           m_stream.SetLength(0);
           if (rema_Len > 0) m_stream.Write(dedata, 0, dedata.Length);
           return ;
         

          
       }

       public byte[] GetData()
       {
           if (m_ListData.Count > 0)
           {
               return m_ListData.Dequeue();
           }
           return null;
       }

    }
}
