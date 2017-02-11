using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//×Ö·û±àÂë²Ù×÷Àà
///2015.8.26
namespace GameCommon
{
    public class Coding
    {
        private static Encoding gb2312 = null;
        private static Encoding utf8 = null;
        private static Encoding latin = null;
        public static Encoding GetDefauleCoding()
        {
            if (gb2312 == null)
            {
                gb2312 = Encoding.GetEncoding("gb2312");
                if (gb2312 == null)
                {
                    Log.WriteLog(Log.LOGTYPE.ERROR, "»ñÈ¡ÏµÍ³Ä¬ÈÏ×Ö·û±àÂëÊ§°Ü,gb2312");
                }
            }
            return gb2312;
        }

        public static Encoding GetLatin1()
        {
            if (latin == null)
            {
                latin = Encoding.GetEncoding("latin1");
                if (latin == null)
                {
                    Log.WriteLog(Log.LOGTYPE.ERROR, "»ñÈ¡latin1×Ö·û±àÂëÊ§°Ü");
                 
                }
            }
            return latin;
        }
        public static Encoding GetUtf8Coding()
        {
            if (utf8 == null)
            {
                utf8 = Encoding.GetEncoding(65001);
                if (utf8 == null)
                {
                    Log.WriteLog(Log.LOGTYPE.ERROR, "»ñÈ¡utf8×Ö·û±àÂëÊ§°Ü");
                }
            }
            return utf8;
        }
        public static String Utf8ToGB2312(byte[] text)
        {
            Init();
            return gb2312.GetString(text);
        }

        public static String Utf8ToGB2312(String text)
        {
            Init();
            byte[] sText = utf8.GetBytes(text);
            return Utf8ToGB2312(sText);
        }

        public static String GB2312ToUtf8(byte[] text)
        {
            Init();
          
            return utf8.GetString(text);
        }

        public static String GB2312ToUtf8(String text)
        {
            Init();
            byte[] sText = gb2312.GetBytes(text);
            return GB2312ToUtf8(sText);
        }


        public static String GB2312ToLatin1(byte[] text)
        {
            Init();
            return latin.GetString(text);
        }
        public static String GB2312ToLatin1(String text)
        {
            Init();
            byte[] sText = gb2312.GetBytes(text);
            return GB2312ToLatin1(sText);
        }
        private static void Init()
        {
            GetDefauleCoding();
            GetUtf8Coding();
            GetLatin1();
        }
    }
}
