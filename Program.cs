// <copyright file="Program.cs" company="Sedat Kapanoglu">
// Copyright © 2016-2020 Sedat Kapanoglu
// SPDX-License-Identifier: MIT
// </copyright>

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;

namespace Unbom
{
    internal static class Program
    {
        private static readonly RootCommand cmd = new RootCommand("Removes UTF-8 BOM markers from text files")
        {
            new Argument<string>("path", "Path to scan"),
            new Option<bool>(new[] { "-r", "--recurse" }, "recurse subdirectories"),
            new Option<bool>(new[] { "-n", "--nobackup" }, "do not save a backup file"),
        };

        public static int Main(string[] args)
        {
            cmd.Handler = CommandHandler.Create<string, bool, bool>(unbom);
            return cmd.Invoke(args);
        }

        private static void unbom(
            string spec,
            bool recurse = false,
            bool nobackup = false)
        {
            Debug.WriteLine($"spec={spec} recurse={recurse} nobackup={nobackup}");
            string? path = Path.GetDirectoryName(spec);
            if (String.IsNullOrEmpty(path))
            {
                path = ".";
            }

            string? pattern = Path.GetFileName(spec);
            if (String.IsNullOrEmpty(pattern))
            {
                pattern = "*";
            }

            var files = Directory.EnumerateFiles(path, pattern, recurse
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly);
            foreach (string fileName in files)
            {
                removeBom(fileName, nobackup);
            }
        }

        private static void removeBom(string fileName, bool nobackup)
        {
            const uint marker = 0xBFBBEF;

            string tempName;
            var buffer = new byte[4];
            using (var stream = File.OpenRead(fileName))
            {
                int bytesRead = stream.Read(buffer, 0, 3);
                if (bytesRead != 3 || marker != BitConverter.ToUInt32(buffer, 0))
                {
                    return;
                }

                Console.Write("{0}: BOM found - removing...", fileName);

                // GetTempFileName also creates the file
                string tempFileName = Path.GetTempFileName();
                using var outputStream = File.Create(tempName = tempFileName);
                stream.CopyTo(outputStream);
            }

            string backupName = fileName + ".bak";
            File.Move(fileName, backupName, overwrite: true);
            File.Move(tempName, fileName);
            if (nobackup)
            {
                File.Delete(backupName);
            }

            Console.WriteLine("done");
        }
    }
}