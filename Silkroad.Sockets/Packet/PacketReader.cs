using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Silkroad.Sockets.Packet
{
    public sealed class PacketReader
    {
        public ushort Opcode { get; }

        private readonly MemoryStream _memoryStream;
        private readonly BinaryReader _binaryReader;
        
        internal PacketReader(byte[] buffer)
        {
            _memoryStream = new MemoryStream(buffer);
            _binaryReader = new BinaryReader(_memoryStream);
            
            ReadUInt16();
            Opcode = ReadUInt16();
            ReadUInt16();
        }

        public bool ReadBoolean()
        {
            return _binaryReader.ReadBoolean();
        }
        
        #region Numbers
        
        public byte ReadUInt8()
        {
            return _binaryReader.ReadByte();
        }

        public byte[] ReadUInt8Array(int count)
        {
            return _binaryReader.ReadBytes(count);
        }

        public ushort ReadUInt16()
        {
            return _binaryReader.ReadUInt16();
        }

        public short ReadInt16()
        {
            return _binaryReader.ReadInt16();
        }
        
        public uint ReadUInt32()
        {
            return _binaryReader.ReadUInt32();
        }

        public int ReadInt32()
        {
            return _binaryReader.ReadInt32();
        }

        public ulong ReadUInt64()
        {
            return _binaryReader.ReadUInt64();
        }

        public long ReadInt64()
        {
            return _binaryReader.ReadInt64();
        }

        public float ReadFloat()
        {
            return _binaryReader.ReadSingle();
        }
        
        #endregion
        
        #region Strings

        public string ReadAscii()
        {
            var length = ReadUInt16();

            var bytes = ReadUInt8Array(length);

            return Encoding.ASCII.GetString(bytes);
        }
        
        public string ReadUnicode()
        {
            var length = ReadUInt16() * 2;

            var bytes = ReadUInt8Array(length);

            return Encoding.Unicode.GetString(bytes);
        }
        
        #endregion

        public byte[] GetBuffer()
        {
            return _memoryStream.ToArray();
        }
    }
}