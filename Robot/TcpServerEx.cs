using HPSocketCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
/*********************
 * tcp服务器同步类 hpsocket.tcpserver 回调的函数是异步，特定封装一个同步类
 * ******************/
namespace GameCommon
{
    
    public class TcpServerEx
    {
        public delegate void OnSocket(SocketEvent e);
        
        private HPSocketCS.TcpServer m_tcpserver;

        private String m_sIP;
        private ushort m_nPort;
        private object m_LockObject;
        private Queue<SocketEvent> m_TempQueue;
        private Queue<SocketEvent> m_Queue;
        
        public OnSocket m_OnSocket;

        public HPSocketCS.TcpServer GetTcpServer()
        {
            return m_tcpserver;
        }
        public TcpServerEx()
        {

            m_tcpserver = new HPSocketCS.TcpServer();
            m_LockObject = new object();
            m_Queue = new Queue<SocketEvent>();
          
            m_TempQueue = new Queue<SocketEvent>();
            
        }

        ~TcpServerEx()
        {


        }

        public void Run()
        {
            //交换数据队列
            lock (m_LockObject)
            {
                if (m_Queue.Count > 0)
                {
                    for (int i = 0; i < m_Queue.Count; i++)
                    {
                        m_TempQueue.Enqueue(m_Queue.Dequeue());
                    }
                }
            }
            if (m_TempQueue.Count > 0)
            {
                while (m_TempQueue.Count > 0)
                {
                    m_OnSocket.Invoke(m_TempQueue.Dequeue());
                }
            }
        }
        public bool Start(String sIp, ushort nPort)
        {
            this.m_sIP = sIp;
            this.m_nPort = nPort;
            m_tcpserver.IpAddress = sIp;
            m_tcpserver.Port = (ushort)nPort;
            m_tcpserver.OnPrepareListen += new TcpServerEvent.OnPrepareListenEventHandler(OnPrepareListen);
            m_tcpserver.OnAccept += new HPSocketCS.TcpServerEvent.OnAcceptEventHandler(OnAccept);
            m_tcpserver.OnSend += new TcpServerEvent.OnSendEventHandler(OnSend);
            m_tcpserver.OnPointerDataReceive += new TcpServerEvent.OnPointerDataReceiveEventHandler(OnPointerDataReceive);
            m_tcpserver.OnClose += new TcpServerEvent.OnCloseEventHandler(OnClose);
            m_tcpserver.OnShutdown += new TcpServerEvent.OnShutdownEventHandler(OnShutdown);

            if (!m_tcpserver.Start())
            {
                Log.WriteLog(Log.LOGTYPE.ERROR, "start tcpserver error!" + sIp + " port:" + nPort.ToString());
                return false;
            }
            return true;
        }

        HandleResult OnPrepareListen(IntPtr soListen)
        {
            // 监听事件到达了,一般没什么用吧?

            return HandleResult.Ok;
        }

        HandleResult OnAccept(IntPtr connId, IntPtr pClient)
        {
            string ip = string.Empty;
            ushort port = 0;
            if (!m_tcpserver.GetRemoteAddress(connId, ref ip, ref port))
            {
                return HandleResult.Error;
            }
            lock (m_LockObject)
            {
                SocketEvent e = new SocketEvent();
                e.connId = connId;
                e.type = SocketEvent.SOCKETEVENT_TYPE.ONACCEPT;
               
                m_Queue.Enqueue(e);
            }

            return HandleResult.Ok;
        }

        HandleResult OnSend(IntPtr connId, byte[] bytes)
        {
            lock (m_LockObject)
            {
                SocketEvent e = new SocketEvent();
                e.type = SocketEvent.SOCKETEVENT_TYPE.ONSEND;
                e.connId = connId;
                e.data = new byte[bytes.Length];
                Buffer.BlockCopy(bytes, 0, e.data, 0, bytes.Length);
                m_Queue.Enqueue(e);
               

            }


            return HandleResult.Ok;
        }

        HandleResult OnPointerDataReceive(IntPtr connId, IntPtr pData, int length)
        {
            lock (m_LockObject)
            {
                SocketEvent e = new SocketEvent();
                e.connId = connId;
                e.data = new byte[length];
                e.type = SocketEvent.SOCKETEVENT_TYPE.ONRECVDATA;
                Marshal.Copy(pData, e.data, 0, length);
                m_Queue.Enqueue(e);
               
            }

            return HandleResult.Ok;
        }

        HandleResult OnClose(IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            lock (m_LockObject)
            {
                SocketEvent e = new SocketEvent();
                e.connId = connId;
                e.type = SocketEvent.SOCKETEVENT_TYPE.ONCLOSE;
                m_Queue.Enqueue(e);
            }
            return HandleResult.Ok;
        }

        HandleResult OnShutdown()
        {
            return HandleResult.Ok;
        }
    }
}
