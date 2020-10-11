using Mackiloha;
using Mackiloha.App.Extensions;
using Mackiloha.Ark;
using Mackiloha.IO;
using Mackiloha.Milo2;
using Mackiloha.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SceneManager.Scene
{
    public class MiloManager
    {
        protected ArkFile Archive;

        public MiloManager()
        {

        }

        public void LoadArk(string hdrPath)
        {
            Archive = ArkFile.FromFile(hdrPath);
        }

        public void LoadMilo(string internalPath)
        {
            internalPath = internalPath.Replace("\\", "/");

            var arkEntry = Archive[internalPath];

            MiloFile miloFile;
            using (var miloStream = Archive.GetArkEntryFileStream(arkEntry))
            {
                miloFile = MiloFile.ReadFromStream(miloStream);
            }

            var info = GetSystemInfo(internalPath, miloFile);
            var serializer = new MiloSerializer(info);

            MiloObjectDir milo;
            using (var miloStream = new MemoryStream(miloFile.Data))
            {
                milo = serializer.ReadFromStream<MiloObjectDir>(miloStream);
            }

            var groups = milo.Entries
                .Where(x => "Group".Equals(x.Type))
                .Select(y => serializer.ReadFromMiloObjectBytes<Group>(y as MiloObjectBytes))
                .ToList();

            var meshes = milo.Entries
                .Where(x => "Mesh".Equals(x.Type))
                .Select(y => serializer.ReadFromMiloObjectBytes<Mesh>(y as MiloObjectBytes))
                .ToList();

            var textures = milo.Entries
                .Where(x => "Tex".Equals(x.Type))
                .Select(y => serializer.ReadFromMiloObjectBytes<Tex>(y as MiloObjectBytes))
                .ToList();
        }

        protected SystemInfo GetSystemInfo(string miloPath, MiloFile miloFile)
            => new SystemInfo()
            {
                Version = miloFile.Version,
                BigEndian = miloFile.BigEndian,
                Platform = miloPath
                    .ToLower()
                    .EndsWith("_ps3")
                    ? Platform.PS3
                    : Platform.X360
            };
    }
}
