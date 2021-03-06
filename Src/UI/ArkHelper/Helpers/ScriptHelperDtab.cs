﻿using ArkHelper.Exceptions;
using CliWrap;
using Mackiloha.DTB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkHelper.Helpers
{
    public class ScriptHelperDtab : IScriptHelper
    {
        protected readonly string DtabPath;

        public ScriptHelperDtab()
        {
            DtabPath = ResolveDtabPath();
        }

        protected virtual string ResolveDtabPath()
        {
            var dtabFileName = $"dtab{GetExeExtension()}";
            var dtabPath = Path.Combine(GetExeDirectory(), dtabFileName);

            // Check if dtab exists relative to exe
            if (File.Exists(dtabPath))
            {
                Console.WriteLine($"Using {dtabFileName} relative to executable");
                return dtabPath;
            }

            Console.WriteLine($"Using {dtabFileName} in Path environment variable");
            return dtabFileName;
        }

        protected virtual string GetExeExtension()
            => RuntimeInformation.OSDescription.Contains("Windows") ? ".exe" : "";

        protected virtual string GetExeDirectory()
            => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        protected virtual void WriteOutput(string text)
            => Console.WriteLine(text);

        public virtual void ConvertNewDtbToOld(string newDtbPath, string oldDtbPath, bool fme = false)
        {
            var encoding = fme ? DTBEncoding.FME : DTBEncoding.RBVR;

            var dtb = DTBFile.FromFile(newDtbPath, encoding);
            dtb.Encoding = DTBEncoding.Classic;
            dtb.SaveToFile(oldDtbPath);
        }

        public virtual string ConvertDtbToDta(string dtbPath, string tempDir, bool newEncryption, int arkVersion, string dtaPath = null)
        {
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var decDtbPath = Path.Combine(tempDir, Path.GetRandomFileName());
            dtaPath = dtaPath ?? Path.Combine(tempDir, Path.GetRandomFileName());

            var dtaDir = Path.GetDirectoryName(dtaPath);
            if (!Directory.Exists(dtaDir))
                Directory.CreateDirectory(dtaDir);

            if (arkVersion < 7)
            {
                // Decrypt dtb
                Cli.Wrap(DtabPath)
                    .SetArguments(new[]
                    {
                        newEncryption ? "-d" : "-D",
                        dtbPath,
                        decDtbPath
                    })
                    .EnableExitCodeValidation(false)
                    .SetStandardOutputCallback(WriteOutput)
                    .SetStandardErrorCallback(WriteOutput)
                    .Execute();
            }
            else
            {
                // New dtb style, convert to old format to use dtab
                //  and assume not encrypted
                ConvertNewDtbToOld(dtbPath, decDtbPath, arkVersion < 9);
            }

            // Convert to dta (plaintext)
            var result = Cli.Wrap(DtabPath)
                .SetArguments(new[]
                {
                    "-a",
                    decDtbPath,
                    dtaPath
                })
                .EnableExitCodeValidation(false)
                .SetStandardOutputCallback(WriteOutput)
                .SetStandardErrorCallback(WriteOutput)
                .Execute();

            if (result.ExitCode != 0)
                throw new DTBParseException($"dtab.exe was unable to parse file from \'{decDtbPath}\'");

            return dtaPath;
        }

        public virtual void ConvertOldDtbToNew(string oldDtbPath, string newDtbPath, bool fme = false)
        {
            var encoding = fme ? DTBEncoding.FME : DTBEncoding.RBVR;

            var dtb = DTBFile.FromFile(oldDtbPath, DTBEncoding.Classic);
            dtb.Encoding = encoding;
            dtb.SaveToFile(newDtbPath);
        }

        public virtual string ConvertDtaToDtb(string dtaPath, string tempDir, bool newEncryption, int arkVersion)
        {
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var dtbPath = Path.Combine(tempDir, Path.GetRandomFileName());
            var encDtbPath = Path.Combine(tempDir, Path.GetRandomFileName());

            // Convert to dtb
            var result = Cli.Wrap(DtabPath)
                .SetArguments(new[]
                {
                    "-b",
                    dtaPath,
                    dtbPath
                })
                .EnableExitCodeValidation(false)
                .SetStandardOutputCallback(WriteOutput)
                .SetStandardErrorCallback(WriteOutput)
                .Execute();

            if (result.ExitCode != 0)
                throw new DTBParseException($"dtab.exe was unable to parse file from \'{dtaPath}\'");

            if (arkVersion < 7)
            {
                // Encrypt dtb (binary)
                Cli.Wrap(DtabPath)
                    .SetArguments(new[]
                    {
                        newEncryption ? "-e" : "-E",
                        dtbPath,
                        encDtbPath
                    })
                    .EnableExitCodeValidation(false)
                    .SetStandardOutputCallback(WriteOutput)
                    .SetStandardErrorCallback(WriteOutput)
                    .Execute();
            }
            else
            {
                // Convert
                ConvertOldDtbToNew(dtbPath, encDtbPath, arkVersion < 9);
            }

            return encDtbPath;
        }
    }
}
