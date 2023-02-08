using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HogwartsLegacy_LocalizationTool
{
    internal class BinaryReader : System.IO.BinaryReader
    {
        public BinaryReader(System.IO.Stream stream) : base(stream) { }

        public string Margic(int size)
        {
            return Encoding.Default.GetString(ReadBytes(size));
        }
        public string ReadStringAtOffset(long offset, int length)
        {
            long pos = base.BaseStream.Position;
            base.BaseStream.Seek(offset,SeekOrigin.Begin);
            string ret = Encoding.UTF8.GetString(base.ReadBytes(length));
            base.BaseStream.Seek(pos, SeekOrigin.Begin);
            return ret;
        }

        public void Skip(int To)
        {
            base.BaseStream.Seek(To, System.IO.SeekOrigin.Current);
        }

        public void Seek(int To)
        {
            base.BaseStream.Seek(To, System.IO.SeekOrigin.Begin);
        }

        public void Seek(uint To)
        {
            base.BaseStream.Seek(To, System.IO.SeekOrigin.Begin);
        }

        public byte[] ReadToEnd()
        {
            return base.ReadBytes((int)base.BaseStream.Length - (int)base.BaseStream.Position);
        }

        public void Pos(int Base)
        {
            base.BaseStream.Position = Base;
        }

        public UInt64[] ReadULonges(int Size)
        {
            List<UInt64> ulongs = new List<UInt64>();
            for(int i = 0; i < Size; i++)
            {
                ulongs.Add(ReadUInt64());
            }
            return ulongs.ToArray();
        }

        public UInt32[] ReadUInts(int Size)
        {
            List<UInt32> uints = new List<UInt32>();
            for (int i = 0; i < Size; i++)
            {
                uints.Add(ReadUInt32());
            }
            return uints.ToArray();
        }

        public long Tell()
        {
            return base.BaseStream.Position;
        }
    }
}
