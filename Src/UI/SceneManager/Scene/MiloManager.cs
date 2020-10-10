using Mackiloha;
using Mackiloha.App;
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

            var groupEntry = milo.Entries.First(x => x.Type == "Group");
            View group;

            using (var groupStream = new MemoryStream((groupEntry as MiloObjectBytes).Data))
            {
                group = serializer.ReadFromStream<View>(groupStream);
            }

            var meshes = new List<Mesh>();
            foreach (var meshName in group.Drawables)
            {
                var meshEntry = milo.Entries.First(x => x.Name == meshName);
                Mesh mesh;

                using (var meshStream = new MemoryStream((meshEntry as MiloObjectBytes).Data))
                {
                    mesh = serializer.ReadFromStream<Mesh>(meshStream);
                }

                meshes.Add(mesh);
            }
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
