using System;
using System.IO;

namespace EsfLibrary
{
    public class AbceCodec : EsfCodec
    {
        public class AbceHeader : EsfHeader
        {
            public uint Unknown1 { get; set; }

            public DateTime EditTime { get; set; }
        }

        private static DateTime UNIX_BASE = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public AbceCodec(uint id = 43982u)
            : base(id)
        {
        }

        public static DateTime GetTime(uint stamp)
        {
            return new DateTime(UNIX_BASE.Ticks).AddSeconds(stamp);
        }

        public static uint GetTimestamp(DateTime time)
        {
            return (uint) time.Subtract(UNIX_BASE).TotalSeconds;
        }

        public override EsfHeader ReadHeader(BinaryReader reader)
        {
            return new AbceHeader
            {
                ID = reader.ReadUInt32(),
                Unknown1 = reader.ReadUInt32(),
                EditTime = GetTime(reader.ReadUInt32())
            };
        }

        public override void WriteHeader(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.Write(0);
            writer.Write(GetTimestamp(DateTime.Now));
        }
    }
}