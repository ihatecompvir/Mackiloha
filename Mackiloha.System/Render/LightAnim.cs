﻿using System;
using System.Collections.Generic;
using System.Text;
using Mackiloha.System.Render.Interfaces;

namespace Mackiloha.System.Render
{
    public struct LightEvent
    {
        public Sphere Origin;
        public float KeyFrame;
    }

    public class LightAnim : RenderObject, IAnim
    {
        public Anim Anim => new Anim();

        public MiloString Light { get; set; }
        public List<LightEvent> Events { get; } = new List<LightEvent>();

        public MiloString LightAnimation { get; set; }
    }
}