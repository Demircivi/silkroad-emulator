using System;

namespace Silkroad.Sockets.Server.Exceptions
{
    public class SocketServerAlreadyStartedException: Exception
    {
        public SocketServerAlreadyStartedException()
            : base("Socket server already started.")
        {
            
        }
    }
}
