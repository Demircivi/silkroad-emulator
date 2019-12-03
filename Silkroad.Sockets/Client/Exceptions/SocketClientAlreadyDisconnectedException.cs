using System;

namespace Silkroad.Sockets.Client.Exceptions
{
    public class SocketClientAlreadyDisconnectedException: Exception
    {
        public SocketClientAlreadyDisconnectedException()
            : base("Socket client already disconnected.")
        {
            
        }
    }
}
