using ImGuiNET;
using OpenTK;
using OpenTK.Core;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SceneManager
{
    public class MainWindow : GameWindow
    {
        ImGuiOverlay Overlay;

        public MainWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
            : base(gameSettings, nativeSettings)
        {
            Title = $"Scene Manager (OpenGL: {GL.GetString(StringName.Version)})";

            Resize += (resized) =>
            {
                GL.Viewport(0, 0, resized.Width, resized.Height);
            };
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            MakeCurrent();

            Overlay = new ImGuiOverlay(this);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Overlay.Update((float)args.Time);

            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            ImGui.ShowDemoWindow();

            Overlay.Render();

            SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            Overlay.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            Overlay.MouseScroll(e.Offset);
        }
    }
}
