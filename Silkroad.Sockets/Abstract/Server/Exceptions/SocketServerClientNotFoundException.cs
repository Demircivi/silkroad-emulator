using System;
using Silkroad.Sockets.Abstract.Client.Models;

namespace Silkroad.Sockets.Abstract.Server.Exceptions
{
    public class SocketServerClientNotFoundException: Exception
    {
        public SocketServerClientNotFoundException(SocketClientId id)
            : base($"Failed to find socket client with id: {id}.")
        {
            
        }
    }
}
