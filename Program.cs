#region Usings

using System;
using System.IO;

#endregion

namespace ClearFlag
{
    internal class Program
    {
        private static int menu { get; set; }

        private static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            WriteFullLine("This utility will clear the read-only and archive attributes on all files in the path you specify.");
            WriteFullLine($"Once changed, this cannot be undone.");
            Console.ResetColor();
            Console.WriteLine(Environment.NewLine);

            menu = DisplayMenu();

            if (menu.Equals(4)) return;

            Console.WriteLine("Please enter folder path:");
            var path = Console.ReadLine();
            if (Directory.Exists(path))
                ProcessDirectory(path);
            else
                Console.WriteLine($@"Invalid folder path entered. {path} does not exist.");
        }

        private static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            var fileEntries = Directory.GetFiles(targetDirectory);
            foreach (var fileName in fileEntries) ProcessFile(fileName);

            var subdirectories = Directory.GetDirectories(targetDirectory);
            foreach (var directory in subdirectories) ProcessDirectory(directory);
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        private static void ProcessFile(string path)
        {
            var attributes = File.GetAttributes(path);

            switch (menu)
            {
                case 1:
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        // Make the file RW
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("The {0} file is no longer Read-Only.", path);
                    }

                    break;

                case 2:
                    if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    {
                        // Clear Archive Flag
                        attributes = RemoveAttribute(attributes, FileAttributes.Archive);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("The {0} file is no longer Archived.", path);
                    }

                    break;
                case 3:
                    if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    {
                        // Clear Archive & RW Flag
                        attributes = RemoveAttribute(attributes, FileAttributes.Archive);
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("The {0} file is no longer Archived or Read-Only.", path);
                    }

                    break;
            }
        }

        public static int DisplayMenu()
        {
            Console.WriteLine("What file attributes would you like to clear?");
            Console.WriteLine();
            Console.WriteLine("1. Read-Only");
            Console.WriteLine("2. Archived");
            Console.WriteLine("3. Read-Only && Archived");
            Console.WriteLine("4. Exit");
            var result = Console.ReadLine();
            return Convert.ToInt32(result);
        }


        static void WriteFullLine(string value)
        {
            Console.WriteLine(value.PadRight(Console.WindowWidth - 1)); // <-- see note
        }
    }
}