using System;
using System.IO;
using Mono.Options;

namespace unbom
{
    class Program
    {
        const uint marker = 0xBFBBEF;

        static bool recurse;
        static bool nobackup;

        static readonly OptionSet options = new OptionSet()
        {
            { "r|recurse", "recurse subdirectories", v => recurse = true },
            { "n|nobackup", "do not save a backup file", v => nobackup = true },
            { "h|?|help", "show this", v => showUsage() }
        };

        static void showUsage()
        {
            Console.Error.WriteLine(@"Removes UTF-8 BOM markers from text files

Usage: unbom [options] <filespec>

Options:");
            options.WriteOptionDescriptions(Console.Error);
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            var extra = options.Parse(args);
            if (extra.Count != 1)
            {
                showUsage();
            }
            string spec = extra[0];
            string? path = Path.GetDirectoryName(spec);
            if (String.IsNullOrEmpty(path))
            {
                path = ".";
            }
            string pattern = Path.GetFileName(spec);
            var files = Directory.EnumerateFiles(path, pattern, recurse 
                ? SearchOption.AllDirectories 
                : SearchOption.TopDirectoryOnly);
            foreach (string fileName in files)
            {
                removeBom(fileName);
            }            
        }

        private static void removeBom(string fileName)
        {
            string tempName;
            using (var stream = File.OpenRead(fileName))
            {
                var buffer = new byte[4];
                int bytesRead = stream.Read(buffer, 0, 3);
                if (bytesRead != 3 || marker != BitConverter.ToUInt32(buffer, 0))
                {
                    return;
                }
                Console.Error.Write("{0}: BOM found - removing...", fileName);

                // GetTempFileName also creates the file
                string tempFileName = Path.GetTempFileName();
                using var outputStream = File.Create(tempName = tempFileName);
                stream.CopyTo(outputStream);
            }
            string backupName = fileName + ".bak";
            if (File.Exists(backupName))
            {
                File.Delete(backupName);
            }
            File.Move(fileName, backupName);
            File.Move(tempName, fileName);
            if (nobackup)
            {
                File.Delete(backupName);
            }
            Console.Error.WriteLine("done");
        }
    }
}