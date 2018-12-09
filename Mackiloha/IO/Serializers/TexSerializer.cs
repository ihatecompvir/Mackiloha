﻿using System;
using System.Collections.Generic;
using System.Text;
using Mackiloha.Render;

namespace Mackiloha.IO.Serializers
{
    public class TexSerializer : AbstractSerializer
    {
        public TexSerializer(MiloSerializer miloSerializer) : base(miloSerializer) { }
        
        public override void ReadFromStream(AwesomeReader ar, ISerializable data)
        {
            var tex = data as Tex;

            int version = ReadMagic(ar, data);

            if (version >= 10)
                ar.BaseStream.Position += 9; // Skips zeros

            tex.Width = ar.ReadInt32();
            tex.Height = ar.ReadInt32();
            tex.Bpp = ar.ReadInt32();

            tex.ExternalPath = ar.ReadString();

            if (ar.ReadSingle() != -8.0f)
                throw new NotSupportedException("TexReader: Expected -8.0");

            if (ar.ReadInt32() != 0x01)
                throw new NotSupportedException($"TexReader: Expected 0x01");

            tex.UseExternal = ar.ReadBoolean();

            tex.Bitmap = MiloSerializer.ReadFromStream<HMXBitmap>(ar.BaseStream);
        }

        public override void WriteToStream(AwesomeWriter aw, ISerializable data)
        {
            var tex = data as Tex;

            // TODO: Add version check
            var version = Magic();
            if (version >= 10)
                aw.Write(new byte[9]);

            aw.Write((int)0x08);

            aw.Write((int)tex.Width);
            aw.Write((int)tex.Height);
            aw.Write((int)tex.Bpp);

            aw.Write(tex.ExternalPath);
            aw.Write((float)-8.0);
            aw.Write((int)0x01);

            if (tex.UseExternal && tex.Bitmap != null)
            {
                aw.Write(true);
                MiloSerializer.WriteToStream(aw.BaseStream, tex.Bitmap);
            }
            else
            {
                aw.Write(false);
            }
        }

        public override bool IsOfType(ISerializable data) => data is Tex;

        public override int Magic()
        {
            switch(MiloSerializer.Info.Version)
            {
                case 10:
                    // GH1
                    return 8;
                case 24:
                    // GH2
                    return 10; // TODO: Take into account other factors for demos
                default:
                    return -1;
            }
        }
    }
}
