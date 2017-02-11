using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    public class BaseSystem
    {
        protected Actor m_pActor;
        protected Session m_pSession;
        public BaseSystem(Actor pActor,Session pSession)
        {
            m_pActor = pActor;
            m_pSession = pSession;
        }
    }
}
