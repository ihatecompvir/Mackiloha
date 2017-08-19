﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mackiloha.Milo
{
    public class Mat : AbstractEntry, IExportable
    {
        public Mat(string name, bool bigEndian = true) : base(name, "", bigEndian)
        {

        }

        public static Mat FromFile(string input)
        {
            using (FileStream fs = File.OpenRead(input))
            {
                return FromStream(fs);
            }
        }
        public static Mat FromStream(Stream input)
        {
            using (AwesomeReader ar = new AwesomeReader(input))
            {
                int version;
                bool valid;
                Mat mat = new Mat("");

                // Guesses endianess
                ar.BigEndian = DetermineEndianess(ar.ReadBytes(4), out version, out valid);
                if (!valid) return null; // Probably do something else later

                int listsTexture = ar.ReadInt32();
                if (listsTexture != 1) return null;

                // Skips to texture name
                ar.BaseStream.Position += 60;
                mat.Texture= ar.ReadString();

                return mat;
            }
        }

        private static bool DetermineEndianess(byte[] head, out int version, out bool valid)
        {
            bool bigEndian = false;
            version = BitConverter.ToInt32(head, 0);
            valid = IsVersionValid(version);

            checkVersion:
            if (!valid && !bigEndian)
            {
                bigEndian = !bigEndian;
                Array.Reverse(head);
                version = BitConverter.ToInt32(head, 0);
                valid = IsVersionValid(version);

                goto checkVersion;
            }

            return bigEndian;
        }

        private static bool IsVersionValid(int version)
        {
            switch (version)
            {
                case 21: // PS2 - GH1
                    return true;
                default:
                    return false;
            }
        }

        public void Import(string path)
        {
            throw new NotImplementedException();
        }

        public void Export(string path)
        {
            throw new NotImplementedException();
        }

        public string Texture { get; set; }

        public override byte[] Data => throw new NotImplementedException();

        public override string Type => "Mat";
    }
}
