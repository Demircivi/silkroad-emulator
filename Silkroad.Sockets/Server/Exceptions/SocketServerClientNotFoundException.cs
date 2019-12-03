using System;
using Silkroad.Sockets.Client.Models;

namespace Silkroad.Sockets.Server.Exceptions
{
    public class SocketServerClientNotFoundException: Exception
    {
        public SocketServerClientNotFoundException(SocketClientId id)
            : base($"Failed to find socket client with id: {id}.")
        {
            
        }
    }
}
