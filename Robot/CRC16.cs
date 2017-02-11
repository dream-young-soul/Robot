using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    public class CRC16
    {
        public static uint POLYNOMIAL = 0x1021;
        private static uint[] CRCTable = MakeCrcTable();
        private static uint[] DropBits = {0xFFFFFFFF, 0xFFFFFFFE, 0xFFFFFFFC, 0xFFFFFFF8, 
			0xFFFFFFF0, 0xFFFFFFE0, 0xFFFFFFC0, 0xFFFFFF80, 
			0xFFFFFF00, 0xFFFFFE00, 0xFFFFFC00, 0xFFFFF800, 
			0xFFFFF000, 0xFFFFE000, 0xFFFFC000, 0xFFFF8000};
        private static uint[] MakeCrcTable()
        {
            uint c;
            uint[] crcTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                c = (uint)((i << 8) & 0xFFFFFF00);
                for (int j = 0; j < 8; j++)
                {
                    c = (c & 0x8000) > 0 ? ((c << 1) & 0xFFFFFFFE) ^ POLYNOMIAL : ((c << 1) & 0xFFFFFFFE);
                }
                crcTable[i] = c;
            }
            return crcTable;
        }

        private static uint CRCBitReflect(uint pInput, uint pBits)
        {

            uint x;
            pBits--;
            uint result = 0;
            for (uint i = 0; i <= pBits; i++)
            {
                x = pBits - i;
                if ((pInput & 1) > 0)
                {
                    result |= (uint)(1 << (int)x) & DropBits[x];
                }
                pInput = (pInput >> 1) & 0x7FFFFFFF;
            }
            return result;
        }

        public static uint Update(byte[] pBytes, uint pOffset = 0, uint pLen = 0)
        {
            pLen = pLen > 0 ? pLen : (uint)pBytes.Length;
            uint c = 0;
            uint index = 0;
            for (uint i = pOffset; i < pLen; i++)
            {
                index = (CRCBitReflect(pBytes[i], 8) & 0xFF) ^ ((c >> 8) & 0x00FFFFFF);
                index = index & 0xFF;
                c = CRCTable[index] ^ ((c << 8) & 0xFFFFFF00);
            }
            uint result = (CRCBitReflect(c, 16) ^ 0) & 0xFFFF;
            return result;
        }
    }
   
   
}
