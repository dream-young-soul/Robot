using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameCommon;
namespace Robot
{
    class Program
    {
        public static Dictionary<IntPtr, Session> g_client;
        private static int startId;
        private static int endId;
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("请指定帐号起始与结束索引!");
                Console.ReadLine();
                return;
            }
            startId = Convert.ToInt32(args[0]);
            endId = Convert.ToInt32(args[1]);
            Thread _thread = new Thread(LogicRun);
            _thread.Start();

            while(true)
            {
                String line = Console.ReadLine();
                switch (line)
                {
                    case "quit":
                        {
                            break;
                        }
                }
            }
            
            
        }

        private static void LogicRun()
        {

            g_client = new Dictionary<IntPtr, Session>();
         
          for (int i = startId; i < endId; i++)
            {

                TcpClientEx client = new TcpClientEx("127.0.0.1", 13006, 5, OnSocket);
                Session s = new Session(client);
                if (client.Connect(true))
                {
                    s.SocketState = SocketState.CHECKING;
                    s.ID = i;
                    s.SendFirstSelfSalt();
                    //s.LoginGame("soul" + i.ToString(), "0000", "魔域");
                    g_client[client.ConnectionId] = s;
                }
      
            }

            while (true)
            {

                Dictionary<IntPtr, Session> dic = new Dictionary<IntPtr, Session>(g_client);

                foreach (Session client in dic.Values)
                {
                    client.Run();

                    
                }
                System.Threading.Thread.Sleep(4);
            }
        }

        private static void OnSocket(ClientSocketEvent e)
        {
            if (!g_client.ContainsKey(e.connId))
            {
                Console.WriteLine("?????????");
                return;
            }
            Session s = g_client[e.connId];
           
            switch (e.type)
            {
                case ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONCLOSE:
                case ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONRECONNECT:
                    {
                        if (g_client.ContainsKey(e.connId))
                        {
                            if (g_client[e.connId].Account == 0)
                            {
                                Console.WriteLine("断开连接!!" + g_client.Count.ToString());
                            }
                            else
                            {
                                Console.WriteLine(string.Format("{0}离开游戏", g_client[e.connId].Account));
                            }
                            
                            g_client.Remove(e.connId);
                        }
                        break;
                    }
                case ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONRECEIVE:
                    {
                        s.OnReceive(e.data);

                        break;
                    }
                case ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONCONNECT:
                    {
                       
                       
                        break;
                    }
             
            }
        }

 
    }
}
