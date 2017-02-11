using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;
using System.Security.Cryptography; 
namespace Robot
{
    //Socket状态
    public enum SocketState
    {
        INIT = 0,       //初始状态
        CONNECTING = 1, //连接状态
        CHECKING = 2,    //校验状态
        COMMUNICATION = 3,//正常通信状态
        DISCONNECT = 4, //断开连接
    }
    public class Session
    {
        public int ID;
        public string Account;
        private GamePackEx m_GamePack;
        private TcpClientEx m_Client;
        private SocketState m_SocketState;
        private Encrypt m_Encrypt;
        private Actor m_Actor;
        public UInt16 HEADER_CHECK_KEY = 0;//包头校验,没发完一个包+1


       
        public SocketState SocketState
        {
            get { return m_SocketState; }
            set { m_SocketState = value; }
        }
    
        public Session(TcpClientEx client)
        {
           
            m_GamePack = new GamePackEx();
            m_SocketState = SocketState.INIT;
            m_Client = client;
            m_Encrypt = new Encrypt();
            m_Actor = new Actor(this);

           
        }
        public GamePackEx GamePack
        {
            get { return m_GamePack; }
            set { m_GamePack = value; }
        }

        
        public void Send(byte[] data)
        {
            byte[] pSendData = this.StructurePack(data);
            m_Client.Send(pSendData);
        }

        public void Run()
        {
            m_Client.Run();
            this.ProcessNetData();
            

        }

        
   

        //连接成功后的第一次发送数据
        public void SendFirstSelfSalt()
        {
            uint seed = m_Encrypt.GetSelfSalt();
            PacketOut pack = new PacketOut();
            pack.WriteUInt32(seed);
            this.m_Client.Send(pack.GetBuffer());
           
        }

        private void SendLoginGame(string sUser,string sPaswd)
        {
            PacketOut pack = new PacketOut();
            pack.WriteCmd(PacketTypes.LOGIN, PacketTypes.LoginPacketTypes.cLogin);
            pack.WriteString(sUser);
            MD5 md5 = MD5.Create();
            byte[] data_paswd = Coding.GetDefauleCoding().GetBytes(sPaswd);
            data_paswd = md5.ComputeHash(data_paswd);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data_paswd.Length; i++)
            {
                sBuilder.Append(data_paswd[i].ToString("x2"));
            }
            sPaswd= sBuilder.ToString();
            pack.WriteString(sPaswd);

            uint gameServerSpanID = 1;
            uint gameServerID = 1;
            const string sign = "ab6e03537c22c1b7f6763178a5882df7";
            const string time = "1445486107";
            const string identity = "000000198010100000";
            pack.WriteUInt32(gameServerSpanID);
            pack.WriteUInt32(gameServerID);
            pack.WriteString(sign);
            pack.WriteString(time);
            pack.WriteString(identity);
            this.Send(pack.GetBuffer());
            Account = sUser;
        }
        public void OnReceive(byte[] data)
        {
            //校验状态
            if (this.SocketState < SocketState.COMMUNICATION)
            {
                this.SocketState = SocketState.COMMUNICATION;
                PackIn pack = new PackIn(data);
                uint salt = pack.ReadUInt32();
                m_Encrypt.SetTargetSalt(salt);
               

                PacketOut pack_out = new PacketOut();
                pack_out.WriteUInt16((UInt16)m_Encrypt.getCheckKey());
                this.m_Client.Send(pack_out.GetBuffer());

                this.SendLoginGame("xb","a");
                return;
            }

            this.GamePack.ProcessNetData(data);
        }

        //构造封包
        public byte[] StructurePack(byte[] pDataBytes)
        {
            uint dataCK = m_Encrypt.GetCRC16(pDataBytes);
            //包头
            PacketOut PackHead = new PacketOut();
            PackHead.WriteUInt16((UInt16)GamePackEx.DEFAULT_TAG);
            PackHead.WriteUInt16((UInt16)pDataBytes.Length);
            PackHead.WriteUInt16((UInt16)dataCK);
            PackHead.WriteUInt16(this.HEADER_CHECK_KEY);
            this.HEADER_CHECK_KEY++;
            if (this.HEADER_CHECK_KEY > 0xFFFF)
            {
                this.HEADER_CHECK_KEY = 0;
            }
            PackHead.WriteInt32(0);
            PackHead.WriteInt32(0);

            //计算包头CRC
            byte[] HeaderBytes = PackHead.GetBuffer();
            uint headerCRC = m_Encrypt.GetCRC16(HeaderBytes);

            byte[] crc_data = BitConverter.GetBytes(headerCRC);
            Buffer.BlockCopy(crc_data, 0, HeaderBytes, 6, sizeof(int));
            m_Encrypt.Encode(HeaderBytes, 4, 4);

            PacketOut pack = new PacketOut();
            pack.WriteBuff(HeaderBytes);
            pack.WriteBuff(pDataBytes);
            return pack.GetBuffer();
        }
        private  void ProcessNetData()
        {
            byte[] data = m_GamePack.GetData();
            if (data != null)
            {
                PackIn packIn = new PackIn(data);
                byte sysId = packIn.ReadByte();
                switch (sysId)
                {
                    case PacketTypes.LOGIN:
                        {
                            m_Actor.GetLoginSystem().ProcessNetData(packIn);
                         
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("未知的系统id:" + sysId + " 协议号:" + packIn.ReadByte());
                            break;
                        }
                }
               
            }
        }
    }
}
