using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HPSocketCS;

namespace GameCommon
{
    public class TcpClientEx
    {
        public delegate void OnSocket(ClientSocketEvent e);

        private OnSocket m_OnSocket;
        private String m_sIP;
        private ushort m_nPort;
        private int m_nReconnectTime;
        private int m_nReconnectTick;
        private HPSocketCS.TcpClient m_TcpClient;
        private bool m_bConnect;
        private bool m_bStartConnect;
        private object m_LockObject;
        private Queue<ClientSocketEvent> m_Queue;
        private Queue<ClientSocketEvent> m_TempQueue;
        public TcpClientEx(String sIP, ushort nPort, int reconnectTime, OnSocket onSocket)
        {
            m_sIP = sIP;
            m_nPort = nPort;
            m_nReconnectTime = reconnectTime;
            m_OnSocket = onSocket;

            m_TcpClient = new HPSocketCS.TcpClient();
            m_bStartConnect = m_bConnect = false;
            m_LockObject = new object();

            m_Queue = new Queue<ClientSocketEvent>();
            m_TempQueue = new Queue<ClientSocketEvent>();
            
            m_TcpClient.OnPrepareConnect += new TcpClientEvent.OnPrepareConnectEventHandler(OnPrepareConnect);
            m_TcpClient.OnConnect += new TcpClientEvent.OnConnectEventHandler(OnConnect);
            m_TcpClient.OnSend += new TcpClientEvent.OnSendEventHandler(OnSend);
            m_TcpClient.OnReceive += new TcpClientEvent.OnReceiveEventHandler(OnReceive);
            m_TcpClient.OnClose += new TcpClientEvent.OnCloseEventHandler(OnClose);
        }

        public string ip
        {
            get { return ip; }
            set { m_sIP = value; }
        }

        public bool ReConnect(String sIP, int nPort)
        {
            if (m_TcpClient != null)
            {
                m_TcpClient.Destroy();
            }

            m_TcpClient = new HPSocketCS.TcpClient();
            m_TcpClient.OnPrepareConnect += new TcpClientEvent.OnPrepareConnectEventHandler(OnPrepareConnect);
            m_TcpClient.OnConnect += new TcpClientEvent.OnConnectEventHandler(OnConnect);
            m_TcpClient.OnSend += new TcpClientEvent.OnSendEventHandler(OnSend);
            m_TcpClient.OnReceive += new TcpClientEvent.OnReceiveEventHandler(OnReceive);
            m_TcpClient.OnClose += new TcpClientEvent.OnCloseEventHandler(OnClose);

            m_Queue.Clear();
            m_TempQueue.Clear();
            this.m_sIP = sIP;
            this.m_nPort = (ushort)nPort;
            return this.Connect(true);
        }
        public bool Connect(bool bOnce = false)
        {
            m_bStartConnect = true;
            m_nReconnectTick = System.Environment.TickCount - m_nReconnectTime;
            if (bOnce)
            {
                if (m_TcpClient.Connect(m_sIP, m_nPort))
                {
                    m_bConnect = true;
                    return true;
                }
                return false;
            }
            return true;
        }
        ~TcpClientEx()
        {

        }

        public void Send(byte[] data)
        {
            if (!m_bConnect) return;
            m_TcpClient.Send(data, data.Length);
        }
        public void Run()
        {
            if (m_bStartConnect && !m_bConnect)
            {
                if (System.Environment.TickCount - m_nReconnectTick > m_nReconnectTime)
                {
                    if (m_TcpClient.Connect(m_sIP, m_nPort))
                    {
                        m_bConnect = true;
                    }
                    else
                    {
                        //永远不会走到这里的，黑人问号脸-。-
                        ClientSocketEvent e = new ClientSocketEvent();
                        e.connId = this.ConnectionId;
                        e.type = ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONRECONNECT;
                        m_OnSocket.Invoke(e);
                    }
                    m_nReconnectTick = System.Environment.TickCount;
                }
            }

           
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
                for (int i = 0; i < m_TempQueue.Count; i++)
                {
                    m_OnSocket.Invoke(m_TempQueue.Dequeue());
                }
            }
           
        }


        
        HandleResult OnPrepareConnect(TcpClient sender, IntPtr socket)
        {
            lock (m_LockObject)
            {
                ClientSocketEvent e = new ClientSocketEvent();
                e.connId = this.ConnectionId;
                e.type = ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONPREPARECONNECT;
                m_Queue.Enqueue(e);
            }
            return HandleResult.Ok;
        }
        public IntPtr ConnectionId { get { return m_TcpClient.ConnectionId; } }
        HandleResult OnConnect(TcpClient sender)
        {
            lock (m_LockObject)
            {
                ClientSocketEvent e = new ClientSocketEvent();
                e.connId = this.ConnectionId;
                e.type = ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONCONNECT;
                m_Queue.Enqueue(e);
            }
          
            return HandleResult.Ok;
        }

        HandleResult OnSend(TcpClient sender, byte[] bytes)
        {
            lock (m_LockObject)
            {
                ClientSocketEvent e = new ClientSocketEvent();
                e.connId = this.ConnectionId;
                e.type = ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONSEND;
                e.data = new byte[bytes.Length];
                Buffer.BlockCopy(bytes, 0, e.data, 0, bytes.Length);
                m_Queue.Enqueue(e);
            }

            return HandleResult.Ok;
        }

        HandleResult OnReceive(TcpClient sender, byte[] bytes)
        {
            lock (m_LockObject)
            {
                ClientSocketEvent e = new ClientSocketEvent();
                e.connId = this.ConnectionId;
                e.type = ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONRECEIVE;
                e.data = new byte[bytes.Length];
                Buffer.BlockCopy(bytes, 0, e.data, 0, bytes.Length);
                m_Queue.Enqueue(e);
            }
            return HandleResult.Ok;
        }

        HandleResult OnClose(TcpClient sender, SocketOperation enOperation, int errorCode)
        {

            lock (m_LockObject)
            {
                m_bConnect = false;
                m_nReconnectTick = System.Environment.TickCount;
                ClientSocketEvent e = new ClientSocketEvent();
                e.connId = this.ConnectionId;
                e.type = ClientSocketEvent.CLIENTSOCKETEVENT_TYPE.ONCLOSE;
                m_Queue.Enqueue(e);
            }
            return HandleResult.Ok;
        }
    }
}
