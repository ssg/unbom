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
        private static readonly byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };

        private static readonly RootCommand cmd =
            new RootCommand("Removes UTF-8 BOM markers from text files")
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
            string path,
            bool recurse = false,
            bool noBackup = false)
        {
            Debug.WriteLine($"path={path} recurse={recurse} nobackup={noBackup}");
            string? directory = Path.GetDirectoryName(path);
            if (String.IsNullOrEmpty(directory))
            {
                directory = ".";
            }

            string? pattern = Path.GetFileName(path);
            if (String.IsNullOrEmpty(pattern))
            {
                pattern = "*";
            }

            Debug.WriteLine($"path={directory} pattern={pattern}");

            var files = Directory.EnumerateFiles(directory, pattern, recurse
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly);
            foreach (string fileName in files)
            {
                removeBom(fileName, noBackup);
            }
        }

        private static void removeBom(string fileName, bool nobackup)
        {
            string tempName;
            var buffer = new byte[3].AsSpan();
            using (var stream = File.OpenRead(fileName))
            {
                int bytesRead = stream.Read(buffer);
                if (bytesRead != buffer.Length || !buffer.SequenceEqual(bom))
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