using System;

namespace Silkroad.Sockets.Abstract.Client.Exceptions
{
    public class SocketClientAlreadyDisconnectedException: Exception
    {
        public SocketClientAlreadyDisconnectedException()
            : base("Socket client already disconnected.")
        {
            
        }
    }
}
