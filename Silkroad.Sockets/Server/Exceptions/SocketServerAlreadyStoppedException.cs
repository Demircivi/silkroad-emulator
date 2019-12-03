using System;

namespace Silkroad.Sockets.Server.Exceptions
{
    public class SocketServerAlreadyStoppedException: Exception
    {
        public SocketServerAlreadyStoppedException()
            : base("Socket server already stopped.")
        {
            
        }
    }
}
