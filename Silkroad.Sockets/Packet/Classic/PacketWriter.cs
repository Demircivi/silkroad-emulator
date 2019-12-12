using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Silkroad.Sockets.Packet.Classic.Exceptions;

namespace Silkroad.Sockets.Packet.Classic
{
    public sealed class PacketWriter
    {
        private readonly MemoryStream _memoryStream;
        private readonly BinaryWriter _binaryWriter;

        private bool _flushed;
        private readonly ushort _opcode;
        private readonly bool _massive;

        public PacketWriter(ushort opcode, bool massive = false)
        {
            _memoryStream = new MemoryStream();
            _binaryWriter = new BinaryWriter(_memoryStream);
            _opcode = opcode;
            _massive = massive;
            
            InitializePacket();
        }

        private void InitializePacket()
        {
            WriteUInt16(0);
            WriteUInt16(_opcode);
            WriteUInt16(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckWritePreconditions()
        {
            if (_flushed)
            {
                throw new PacketWriterFlushedCantWriteException();
            }
        }

        public void WriteBoolean(bool value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }
        
        #region Numbers

        public void WriteUInt8(byte value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteUInt8Array(byte[] values)
        {
            // NOTE: Remove this precondition maybe?
            // I didn't remove this because when I call this method with an empty array it won't throw any exceptions
            CheckWritePreconditions();

            foreach (var value in values)
            {
                WriteUInt8(value);
            }
        }

        public void WriteInt8(sbyte value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteUInt16(ushort value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteInt16(short value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteUInt32(uint value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteInt32(int value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteUInt64(ulong value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteInt64(long value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        public void WriteFloat(float value)
        {
            CheckWritePreconditions();

            _binaryWriter.Write(value);
        }

        #endregion

        #region Strings

        public void WriteAscii(string value)
        {
            CheckWritePreconditions();

            WriteUInt16((ushort) value.Length);

            var bytes = Encoding.ASCII.GetBytes(value);
            WriteUInt8Array(bytes);
        }

        public void WriteUnicode(string value)
        {
            CheckWritePreconditions();

            WriteUInt16((ushort) value.Length);

            var bytes = Encoding.Unicode.GetBytes(value);
            WriteUInt8Array(bytes);
        }

        #endregion

        public byte[] GetBytes()
        {
            if (_flushed)
            {
                throw new PacketWriterFlushedCantGetBytesException();
            }
            
            try
            {
                if (_massive)
                {
                    return GetBytesForMassive();
                }
                
                return GetBytesDefault();
            }
            finally
            {
                _flushed = true;
            }
        }

        private byte[] GetBytesForMassive()
        {
            // TODO: Support massive for real, split packets into sub packets.

            // Get payload
            var payload = GetBytesDefault().Skip(6).ToArray();
            
            // Header packet
            var headerPacketWriter = new PacketWriter(0x600d);

            headerPacketWriter.WriteBoolean(true);
            headerPacketWriter.WriteUInt16(1);
            headerPacketWriter.WriteUInt16(_opcode);
            
            // Body packet
            var bodyPacketWriter = new PacketWriter(0x600d);
            
            bodyPacketWriter.WriteBoolean(false);
            bodyPacketWriter.WriteUInt8Array(payload);

            return headerPacketWriter.GetBytes().Concat(bodyPacketWriter.GetBytes()).ToArray();
        }

        private byte[] GetBytesDefault()
        {
            _memoryStream.Position = 0;
            WriteUInt16((ushort) (_memoryStream.Length - 6));

            return _memoryStream.ToArray();
        }
    }
}
