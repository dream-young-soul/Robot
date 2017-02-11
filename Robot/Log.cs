
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCommon
{
    public class Log
    {
        //��־����
        public enum LOGTYPE
        {
            NORMAL = 0,  
            WARNING = 1, //����
            ERROR = 2,   //����
            FAILED = 3, //ʧ��
        }
       
        public static void WriteLog(LOGTYPE type,Object message)
        {
            Console.WriteLine(message);
           
        }
    }
}
