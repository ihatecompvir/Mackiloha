using ImGuiNET;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SceneManager
{
    public class ImGuiOverlay : IDisposable
    {
        protected readonly NativeWindow Window;

        protected int Width;
        protected int Height;

        protected bool FrameStarted;

        protected int VertexArray;
        protected int VertexBuffer;
        protected int VertexBufferSize;
        protected int IndexBuffer;
        protected int IndexBufferSize;

        protected Texture FontTexture;
        protected Shader Shader;

        protected MouseState PrevMouseState;
        protected KeyboardState PrevKeyboardState;
        protected readonly List<char> PressedChars = new List<char>();

        public ImGuiOverlay(NativeWindow window)
        {
            (Window, Width, Height) = (window, window.ClientSize.X, window.ClientSize.Y);

            Window.Resize += (resized) =>
            {
                (Width, Height) = (resized.Width, resized.Height);
            };

            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var io = ImGui.GetIO();
            io.Fonts.AddFontDefault();

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            InitResources();
            SetKeyMappings();

            SetPerFrameData(1.0f / 60.0f);
            ImGui.NewFrame();
            FrameStarted = true;
            return;

            //ImGui.Render();

            ImGui.BeginMainMenuBar();

            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open")) ;

                ImGui.Separator();
                if (ImGui.MenuItem("Save As")) ;

                ImGui.Separator();
                if (ImGui.MenuItem("Exit"))
                    Environment.Exit(0);

                ImGui.EndMenu();
            }

            ImGui.MenuItem("Options");
            ImGui.MenuItem("Help");

            ImGui.EndMainMenuBar();
        }

        protected void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
        }

        protected virtual void InitResources()
        {
            Utilities.CreateVertexArray("ImGui", out VertexArray);

            VertexBufferSize = 10000;
            IndexBufferSize = 2000;

            Utilities.CreateVertexBuffer("ImGui", out VertexBuffer);
            Utilities.CreateElementBuffer("ImGui", out IndexBuffer);
            GL.NamedBufferData(VertexBuffer, VertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.NamedBufferData(IndexBuffer, IndexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            RecreateFontDeviceTexture();

            var vertexData = Utilities.ReadEmbeddedFile("Shaders/Vertex.glsl");
            var fragmentData = Utilities.ReadEmbeddedFile("Shaders/Fragment.glsl");

            Shader = new Shader("ImGui", vertexData, fragmentData);

            GL.VertexArrayVertexBuffer(VertexArray, 0, VertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            GL.VertexArrayElementBuffer(VertexArray, IndexBuffer);

            GL.EnableVertexArrayAttrib(VertexArray, 0);
            GL.VertexArrayAttribBinding(VertexArray, 0, 0);
            GL.VertexArrayAttribFormat(VertexArray, 0, 2, VertexAttribType.Float, false, 0);

            GL.EnableVertexArrayAttrib(VertexArray, 1);
            GL.VertexArrayAttribBinding(VertexArray, 1, 0);
            GL.VertexArrayAttribFormat(VertexArray, 1, 2, VertexAttribType.Float, false, 8);

            GL.EnableVertexArrayAttrib(VertexArray, 2);
            GL.VertexArrayAttribBinding(VertexArray, 2, 0);
            GL.VertexArrayAttribFormat(VertexArray, 2, 4, VertexAttribType.UnsignedByte, true, 16);

            Utilities.CheckGLError("End InitResources");
        }

        protected void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            FontTexture = new Texture("ImGui Text Atlas", width, height, pixels);
            FontTexture.SetMagFilter(TextureMagFilter.Linear);
            FontTexture.SetMinFilter(TextureMinFilter.Linear);

            io.Fonts.SetTexID((IntPtr)FontTexture.GLTexture);

            io.Fonts.ClearTexData();
        }

        protected virtual void SetPerFrameData(float deltaSeconds)
        {
            var io = ImGui.GetIO();

            io.DisplaySize = new System.Numerics.Vector2(Width, Height);
            io.DisplayFramebufferScale = System.Numerics.Vector2.One;
            io.DeltaTime = deltaSeconds;
        }

        public virtual void Render()
        {
            if (!FrameStarted)
                return;

            ImGui.BeginMainMenuBar();

            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open")) ;

                ImGui.Separator();
                if (ImGui.MenuItem("Save As")) ;

                ImGui.Separator();
                if (ImGui.MenuItem("Exit"))
                    Environment.Exit(0);

                ImGui.EndMenu();
            }

            ImGui.MenuItem("Options");
            ImGui.MenuItem("Help");

            ImGui.EndMainMenuBar();

            FrameStarted = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
        }

        protected void RenderImDrawData(ImDrawDataPtr drawData)
        {
            if (drawData.CmdListsCount == 0)
            {
                return;
            }

            for (int i = 0; i < drawData.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = drawData.CmdListsRange[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > VertexBufferSize)
                {
                    int newSize = (int)Math.Max(VertexBufferSize * 1.5f, vertexSize);
                    GL.NamedBufferData(VertexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    VertexBufferSize = newSize;

                    Console.WriteLine($"Resized dear imgui vertex buffer to new size {VertexBufferSize}");
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > IndexBufferSize)
                {
                    int newSize = (int)Math.Max(IndexBufferSize * 1.5f, indexSize);
                    GL.NamedBufferData(IndexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    IndexBufferSize = newSize;

                    Console.WriteLine($"Resized dear imgui index buffer to new size {IndexBufferSize}");
                }
            }

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
                0.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            Shader.UseShader();
            GL.UniformMatrix4(Shader.GetUniformLocation("projection_matrix"), false, ref mvp);
            GL.Uniform1(Shader.GetUniformLocation("in_fontTexture"), 0);
            Utilities.CheckGLError("Projection");

            GL.BindVertexArray(VertexArray);
            Utilities.CheckGLError("VAO");

            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            // Render command lists
            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = drawData.CmdListsRange[n];

                GL.NamedBufferSubData(VertexBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
                Utilities.CheckGLError($"Data Vert {n}");

                GL.NamedBufferSubData(IndexBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);
                Utilities.CheckGLError($"Data Idx {n}");

                int vtx_offset = 0;
                int idx_offset = 0;

                for (int cmdI = 0; cmdI < cmd_list.CmdBuffer.Size; cmdI++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmdI];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);
                        Utilities.CheckGLError("Texture");

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, Height - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                        Utilities.CheckGLError("Scissor");

                        if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(idx_offset * sizeof(ushort)), vtx_offset);
                        }
                        else
                        {
                            GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                        Utilities.CheckGLError("Draw");
                    }

                    idx_offset += (int)pcmd.ElemCount;
                }
                vtx_offset += cmd_list.VtxBuffer.Size;
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
        }

        public virtual void Update(float deltaSeconds)
        {
            if (FrameStarted)
            {
                ImGui.Render();
            }

            SetPerFrameData(deltaSeconds);
            UpdateImGuiInput();

            FrameStarted = true;
            ImGui.NewFrame();
        }

        protected void UpdateImGuiInput()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            MouseState MouseState = Window.MouseState;
            KeyboardState KeyboardState = Window.KeyboardState;

            io.MouseDown[0] = MouseState[MouseButton.Left];
            io.MouseDown[1] = MouseState[MouseButton.Right];
            io.MouseDown[2] = MouseState[MouseButton.Middle];

            var screenPoint = new Vector2i((int)MouseState.X, (int)MouseState.Y);
            var point = screenPoint;//wnd.PointToClient(screenPoint);
            io.MousePos = new System.Numerics.Vector2(point.X, point.Y);

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.Unknown)
                {
                    continue;
                }
                io.KeysDown[(int)key] = KeyboardState.IsKeyDown(key);
            }

            foreach (var c in PressedChars)
            {
                io.AddInputCharacter(c);
            }
            PressedChars.Clear();

            io.KeyCtrl = KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);
            io.KeyAlt = KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt);
            io.KeyShift = KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
            io.KeySuper = KeyboardState.IsKeyDown(Keys.LeftSuper) || KeyboardState.IsKeyDown(Keys.RightSuper);

            PrevMouseState = MouseState;
            PrevKeyboardState = KeyboardState;
        }


        public void PressChar(char keyChar)
        {
            PressedChars.Add(keyChar);
        }

        public void MouseScroll(Vector2 offset)
        {
            ImGuiIOPtr io = ImGui.GetIO();

            io.MouseWheel = offset.Y;
            io.MouseWheelH = offset.X;
        }

        public void Dispose()
        {
            FontTexture?.Dispose();
            Shader?.Dispose();
        }
    }
}
