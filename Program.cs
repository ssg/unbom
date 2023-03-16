// <copyright file="Program.cs" company="Sedat Kapanoglu">
// Copyright © 2016-2020 Sedat Kapanoglu
// SPDX-License-Identifier: MIT
// </copyright>

using System;
using System.Diagnostics;
using System.IO;

namespace Unbom
{
    internal static class Program
    {
        private static readonly byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };

        /// <summary>
        /// Removes UTF-8 BOM markers from text files.
        /// </summary>
        /// <param name="argument">Path to scan.</param>
        /// <param name="recurse">recurse subdirectories.</param>
        /// <param name="nobackup">do not save a backup file.</param>
        public static void Main(string argument, bool recurse = false, bool nobackup = true)
        {
            unbom(argument, recurse, nobackup);
        }

        private static void unbom(
            string path,
            bool recurse = false,
            bool noBackup = false)
        {
            Debug.WriteLine($"path={path} recurse={recurse} nobackup={noBackup}");

            string? pattern = Path.GetFileName(path);
            if (String.IsNullOrEmpty(pattern))
            {
                pattern = "*";
            }
            else
            {
                path = Path.GetDirectoryName(path) ?? ".";
            }

            Debug.WriteLine($"path={path} pattern={pattern}");

            try
            {
                var files = Directory.EnumerateFiles(path, pattern, recurse
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly);
                int count = 0;
                foreach (string fileName in files)
                {
                    removeBom(fileName, noBackup);
                    count++;
                }
                Console.WriteLine($"{count} file(s) processed");
            }
            catch (DirectoryNotFoundException)
            {
                Console.Error.WriteLine($"Directory not found: {path}");
            }
        }

        private static void removeBom(string fileName, bool nobackup)
        {
            string tempName;
            var buffer = new byte[bom.Length].AsSpan();
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