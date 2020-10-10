using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SceneManager
{
    public static class Utilities
    {
        [Pure]
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [Conditional("DEBUG")]
        public static void CheckGLError(string title)
        {
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Debug.Print($"{title}: {error}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateProgram(string name, out int program)
        {
            program = GL.CreateProgram();
            LabelObject(ObjectLabelIdentifier.Program, program, $"Program: {name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateShader(ShaderType type, string name, out int shader)
        {
            shader = GL.CreateShader(type);
            LabelObject(ObjectLabelIdentifier.Shader, shader, $"Shader: {type}: {name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateTexture(TextureTarget target, string name, out int texture)
        {
            GL.CreateTextures(target, 1, out texture);
            LabelObject(ObjectLabelIdentifier.Texture, texture, $"Texture: {name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
        {
            GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateBuffer(string name, out int buffer)
        {
            GL.CreateBuffers(1, out buffer);
            LabelObject(ObjectLabelIdentifier.Buffer, buffer, $"Buffer: {name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateVertexBuffer(string name, out int buffer)
        {
            CreateBuffer($"VBO: {name}", out buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateElementBuffer(string name, out int buffer)
        {
            CreateBuffer($"EBO: {name}", out buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateVertexArray(string name, out int vao)
        {
            GL.CreateVertexArrays(1, out vao);
            LabelObject(ObjectLabelIdentifier.VertexArray, vao, $"VAO: {name}");
        }

        public static string ReadEmbeddedFile(string path)
        {
            var slashRegex = new Regex("(?i)[\\/]");

            var assembly = typeof(Utilities).Assembly;
            var fullPath = $"{assembly.GetName().Name}.{slashRegex.Replace(path, ".")}";

            var embeddedPath = assembly
                .GetManifestResourceNames()
                .Single(x => x.Equals(fullPath, StringComparison.CurrentCultureIgnoreCase));

            using var stream = assembly.GetManifestResourceStream(embeddedPath);

            using var sr = new StreamReader(stream, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}
