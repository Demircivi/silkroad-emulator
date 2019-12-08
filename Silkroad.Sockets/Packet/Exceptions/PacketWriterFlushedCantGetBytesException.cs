using System;

namespace Silkroad.Sockets.Packet.Exceptions
{
    public class PacketWriterFlushedCantGetBytesException: Exception
    {
        public PacketWriterFlushedCantGetBytesException()
            : base("Can't get bytes because packet writer is flushed.")
        {
        }
    }
}