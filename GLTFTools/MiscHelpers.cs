using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace GLTFTools
{
    public static class MiscHelpers
    {
        private static string GetMimeType(string ext)
        {
            ext = ext.ToLower();

            switch (ext)
            {
                case "glb":
                    return "model/gltf-binary";
                case "gltf":
                    return "model/gltf+json";
                case "bin":
                    return "application/octet-stream";
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                case "json":
                    return "application/json";
                default:
                    // text/plain;charset=US-ASCII
                    return null;
            }
        }

        private static string GetBase64Encoding(string path)
        {
            try
            {
                if (!File.Exists(path)) return "";

                using (var fs = File.OpenRead(path))
                {
                    if (fs.Length > 0x3200000) return ""; // ~50MB

                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);

                    return Convert.ToBase64String(data);
                }
            }
            catch
            {
                return "";
            }
        }

        private static string GetBase64Encoding(Stream stream)
        {
            try
            {
                var size = stream.Length - stream.Position;
                if (size > 0x3200000) return ""; // ~50MB

                byte[] data = new byte[size];
                stream.Read(data, 0, data.Length);

                return Convert.ToBase64String(data);
            }
            catch
            {
                return "";
            }
        }

        private static string GetFileExtension(string path)
        {
            try
            {
                return Path.GetExtension(path).Substring(1);
            }
            catch
            {
                return "";
            }
        }

        public static string EncodeFilePathAsURI(string path)
        {
            return "";
        }

        public static string EncodeFileAsDataURI(string path, bool encodeBase64 = false)
        {
            // RFC 2397
            var data = encodeBase64 ? $";base64,{GetBase64Encoding(path)}" : ",";
            return $"data:{GetMimeType(GetFileExtension(path))}{data}";
        }

        public static string EncodeStreamAsDataURI(Stream stream, string ext, bool encodeBase64 = false)
        {
            // RFC 2397
            var data = encodeBase64 ? $";base64,{GetBase64Encoding(stream)}" : ",";
            return $"data:{GetMimeType(ext)}{data}";
        }
    }
}
