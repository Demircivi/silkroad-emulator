namespace Silkroad.Sockets.Abstract.Client.Models
{
    public struct SocketClientId
    {
        private static readonly object Lock = new object();
        private static ulong _lastId = 0;

        private readonly ulong _id;

        private SocketClientId(ulong id)
        {
            _id = id;
        }

        internal static SocketClientId New()
        {
            lock (Lock)
            {
                return new SocketClientId(_lastId++);
            }
        }

        public bool Equals(SocketClientId other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return obj is SocketClientId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{_id}";
        }

        public static bool operator ==(SocketClientId left, SocketClientId right)
        {
            return left.Equals(right);              
        }
        
        public static bool operator !=(SocketClientId left, SocketClientId right)
        {
            return !left.Equals(right);              
        }
    }
}
