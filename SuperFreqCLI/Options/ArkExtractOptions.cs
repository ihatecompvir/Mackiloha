﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CliWrap;
using CommandLine;
using Mackiloha;
using Mackiloha.Ark;
using SuperFreqCLI.Exceptions;

namespace SuperFreqCLI.Options
{
    [Verb("arkextract", HelpText = "Extracts files from milo arks", Hidden = true)]
    public class ArkExtractOptions
    {
        [Value(0, Required = true, MetaName = "arkPath", HelpText = "Path to ark (hdr file)")]
        public string InputPath { get; set; }

        [Value(1, Required = true, MetaName = "dirPath", HelpText = "Path to output directory")]
        public string OutputPath { get; set; }

        [Option('s', "convertScripts", HelpText = "Convert dtb scripts to dta")]
        public bool ConvertScripts { get; set; }

        [Option('a', "extractAll", HelpText = "Extract everything")]
        public bool ExtractAll { get; set; }

        private static void WriteOutput(string text)
            => Console.WriteLine(text);

        private static string CreateDTAFile(string dtbPath, string tempDir, bool newEncryption, string dtaPath = null)
        {
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var decDtbPath = Path.Combine(tempDir, Path.GetRandomFileName());
            dtaPath = dtaPath ?? Path.Combine(tempDir, Path.GetRandomFileName());

            var dtaDir = Path.GetDirectoryName(dtaPath);
            if (!Directory.Exists(dtaDir))
                Directory.CreateDirectory(dtaDir);

            // Convert to dtb
            Cli.Wrap("dtab")
                .SetArguments(new[]
                {
                    newEncryption ? "-d" : "-D",
                    dtbPath,
                    decDtbPath
                })
                .Execute();

            // Encrypt dtb
            var result = Cli.Wrap("dtab")
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

        private static string CombinePath(string basePath, string path)
        {
            // Consistent slash
            basePath = (basePath ?? "").Replace("/", "\\");
            path = (path ?? "").Replace("/", "\\");

            path = ReplaceDotsInPath(path);
            return Path.Combine(basePath, path);
        }

        private static string ReplaceDotsInPath(string path)
        {
            var dotRegex = new Regex(@"[.]+[\/\\]");

            if (dotRegex.IsMatch(path))
            {
                // Replaces dotdot path
                path = dotRegex.Replace(path, x => $"({x.Value.Substring(0, x.Value.Length - 1)}){x.Value.Last()}");
            }

            return path;
        }

        private static string ExtractEntry(ArkFile ark, ArkEntry entry, string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var stream = ark.GetArkEntryFileStream(entry))
                {
                    stream.CopyTo(fs);
                }
            }

            return filePath;
        }

        public static void Parse(ArkExtractOptions op)
        {
            var scriptRegex = new Regex("(?i).((dtb)|(dta))$");
            var dtbRegex = new Regex("(?i).dtb$");
            var genPathedFile = new Regex(@"(?i)(([^\/\\]+[\/\\])*)(gen[\/\\])([^\/\\]+)$");

            var ark = ArkFile.FromFile(op.InputPath);

            var scriptsToConvert = ark.Entries
                .Where(x => op.ConvertScripts
                    && scriptRegex.IsMatch(x.FullPath))
                .ToList();

            var entriesToExtract = ark.Entries
                .Where(x => op.ExtractAll)
                .Except(scriptsToConvert)
                .ToList();

            foreach (var arkEntry in entriesToExtract)
            {
                var filePath = ExtractEntry(ark, arkEntry, CombinePath(op.OutputPath, arkEntry.FullPath));
                Console.WriteLine($"Wrote \"{filePath}\"");
            }

            // Create temp path
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);


            var successDtas = 0;
            foreach (var scriptEntry in scriptsToConvert)
            {
                if (!dtbRegex.IsMatch(scriptEntry.FullPath))
                {
                    var filePath = ExtractEntry(ark, scriptEntry, CombinePath(op.OutputPath, scriptEntry.FullPath));
                    Console.WriteLine($"Wrote \"{filePath}\"");
                    continue;
                }

                // Creates output path
                var dtaPath = CombinePath(op.OutputPath, scriptEntry.FullPath);
                dtaPath = $"{dtaPath.Substring(0, dtaPath.Length - 1)}a";

                // Removes gen sub directory
                if (genPathedFile.IsMatch(dtaPath))
                {
                    var match = genPathedFile.Match(dtaPath);
                    dtaPath = $"{match.Groups[1]}{match.Groups[4]}";
                }

                var tempDtbPath = ExtractEntry(ark, scriptEntry, Path.Combine(tempDir, Path.GetRandomFileName()));

                try
                {
                    CreateDTAFile(tempDtbPath, tempDir, ark.Encrypted, dtaPath);
                    Console.WriteLine($"Wrote \"{dtaPath}\"");
                    successDtas++;
                }
                catch (DTBParseException ex)
                {
                    Console.WriteLine($"Unable to convert to script, skipping \'{scriptEntry.FullPath}\'");
                    if (File.Exists(dtaPath))
                        File.Delete(dtaPath);
                }
                catch (Exception ex)
                {

                }
            }

            if (scriptsToConvert.Count > 0)
                Console.WriteLine($"Converted {successDtas} of {scriptsToConvert.Count} scripts");

            // Clean up temp files
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}