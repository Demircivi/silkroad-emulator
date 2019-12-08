using System;

namespace Silkroad.Sockets.Abstract.Server.Exceptions
{
    public class SocketServerAlreadyStoppedException: Exception
    {
        public SocketServerAlreadyStoppedException()
            : base("Socket server already stopped.")
        {
            
        }
    }
}
