using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    public class Actor
    {
        public LoginSystem m_pLoginSystem;
        private TSimplePlayerInfo m_SimplePlayerInfo;
        private Session m_pSession;
        public Actor(Session pSession)
        {
            m_pSession = pSession;
            m_pLoginSystem = new LoginSystem(this, m_pSession);
        }

        public LoginSystem GetLoginSystem() { return m_pLoginSystem; }

        public  TSimplePlayerInfo SimplePlayerInfo
        {
            get
            {
                return m_SimplePlayerInfo;
            }
            set
            {
               m_SimplePlayerInfo = value;
            }
         
        }
        
    }
}
