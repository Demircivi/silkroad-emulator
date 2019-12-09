using System;

namespace Silkroad.Sockets.Abstract.Client.Exceptions
{
    public class SocketClientTimeoutException: Exception
    {
        public SocketClientTimeoutException()
            : base("Socket disconnected because of a timeout.")
        {
            
        }
    }
}
