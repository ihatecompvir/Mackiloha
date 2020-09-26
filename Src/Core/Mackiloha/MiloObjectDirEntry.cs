﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mackiloha
{
    public class MiloObjectDirEntry
    {
        public int Version { get; set; } // 22
        public int SubVersion { get; set; } // 2
        public string Name { get; set; }
        public string[] ImportedMiloPaths { get; set; }
        public List<MiloObjectDir> SubDirectories { get; set; } = new List<MiloObjectDir>();
    }
}