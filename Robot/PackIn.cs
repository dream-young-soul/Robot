using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
//流读入
//2015.8.5
namespace GameCommon
{
    public class PackIn
    {
        private MemoryStream stream;
        private BinaryReader read;
        public PackIn(byte[] data)
        {
            stream = new MemoryStream(data);
            read = new BinaryReader(stream);
        }
        ~PackIn()
        {
            stream.Close();
            read = null;
           
        }
        public int ReadInt32()
        {
            if (this.IsComplete(sizeof(Int32))) return 0;
            return read.ReadInt32();
          
        }

        public uint ReadUInt32()
        {
            if (this.IsComplete(sizeof(UInt32))) return 0;
            return read.ReadUInt32();
        }

        public short ReadInt16()
        {
            if (this.IsComplete(sizeof(Int16))) return 0;
            return read.ReadInt16();
        }

        public ushort ReadUInt16()
        {
            if (this.IsComplete(sizeof(UInt16))) return 0;
            return read.ReadUInt16();
        }

        public long ReadLong()
        {
            if (this.IsComplete(sizeof(long))) return 0;
            return read.ReadInt64();
        }

        public ulong ReadULong()
        {
            if (this.IsComplete(sizeof(ulong))) return 0;
            return read.ReadUInt64();
        }
        public bool ReadBool()
        {
            if (this.IsComplete(sizeof(bool))) return false;
            return read.ReadBoolean();
        }
        public byte[] ReadBuff(int len,bool readLen = false)
        {

            if (this.IsComplete(len)) return null;
            if (readLen)
            {
                len += sizeof(ushort);
                read.BaseStream.Seek(read.BaseStream.Position - sizeof(ushort), SeekOrigin.Begin);
            }
            byte[] buf = read.ReadBytes(len);
            return buf;
        }
        public float ReadFloat()
        {
            if (this.IsComplete(sizeof(float))) return 0;
            return read.ReadSingle();
        }
        public String ReadString()
        {
            if (this.IsComplete(sizeof(byte))) return "";
            byte nLen = read.ReadByte();
            byte hi_ = read.ReadByte();
            if (hi_ > 0)
            {
                nLen = (byte)MAKEWORD(nLen, hi_);
            }
            if (this.IsComplete(nLen)) return "";
            byte[] buf = read.ReadBytes(nLen);
            return Coding.GetDefauleCoding().GetString(buf);
        }
        public int MAKEWORD(byte a, byte b)
        {
            return ((ushort)(((byte)(((int)(a)) & 0xff)) | ((ushort)((byte)(((int)(b)) & 0xff))) << 8));
        }
        public String ReadString(int len)
        {
            if (read.BaseStream.Position + len > read.BaseStream.Length) return "";
            byte[] buf = read.ReadBytes(len);
            String str = Coding.GetDefauleCoding().GetString(buf);
            int npos = str.IndexOf('\0');
            if (npos >= 0)
            {
                str = str.Substring(0, npos);
            }
            return str;
        }

        public byte ReadByte()
        {
            if (this.IsComplete(sizeof(byte))) return 0;
            return read.ReadByte();
           
        }

        public bool IsComplete(int nLen)
        {
            return read.BaseStream.Position + nLen > stream.Length;
        }
    }
}
