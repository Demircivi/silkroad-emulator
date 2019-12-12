using System;

namespace Silkroad.Sockets.Packet.Classic.Exceptions
{
    public class PacketWriterFlushedCantGetBytesException: Exception
    {
        public PacketWriterFlushedCantGetBytesException()
            : base("Can't get bytes because packet writer is flushed.")
        {
        }
    }
}