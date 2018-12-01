﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mackiloha.IO
{
    public partial class MiloSerializer
    {
        private static byte[] ADDE_PADDING = { 0xAD, 0xDE, 0xAD, 0xDE }; // Used to pad files

        private void ReadFromStream(AwesomeReader ar, MiloObjectDir dir)
        {
            // TODO: Add version check
            if (ar.ReadInt32() != 0x0A)
                throw new NotSupportedException($"MiloObjectReader: Expected 0x0A at offset 0");

            int entryCount = ar.ReadInt32();
            var entries = Enumerable.Range(0, entryCount).Select(x => new
            {
                Type = ar.ReadString(),
                Name = ar.ReadString()
            }).ToArray();

            // Skips external resource paths?
            entryCount = ar.ReadInt32();
            for (int i = 0; i < entryCount; i++) ar.ReadString();

            foreach (var entry in entries)
            {
                var entryOffset = ar.BaseStream.Position;
                
                try
                {
                    var miloEntry = ReadFromStream(ar.BaseStream, entry.Type);
                    miloEntry.Name = entry.Name;

                    dir.Entries.Add(miloEntry);
                    ar.BaseStream.Position += 4; // Skips padding
                }
                catch (Exception ex)
                {
                    ar.BaseStream.Position = entryOffset;
                    int magic;

                    do
                    {
                        int size = (int)ar.FindNext(ADDE_PADDING);
                        if (size == -1)
                        {
                            ar.BaseStream.Seek(0, SeekOrigin.End);
                            break; // End of file reached!
                        }

                        ar.BaseStream.Position += 4; // Skips padding
                        
                        if (ar.BaseStream.Position >= ar.BaseStream.Length)
                        {
                            // EOF reached
                            break;
                        }

                        // Checks magic because ADDE padding can also be found in some Tex files as pixel data
                        // This should reduce false positives
                        magic = ar.ReadInt32();
                        ar.BaseStream.Position -= 4;

                    } while (magic < 0 || magic > 0xFF);
                    

                    // Reads data as a byte array
                    var entrySize = ar.BaseStream.Position - (entryOffset + 4);
                    ar.BaseStream.Position = entryOffset;

                    var entryBytes = new MiloObjectBytes(entry.Type) { Name = entry.Name };
                    entryBytes.Data = ar.ReadBytes((int)entrySize);
                    dir.Entries.Add(entryBytes);

                    ar.BaseStream.Position += 4;
                }
            }
        }
    }
}