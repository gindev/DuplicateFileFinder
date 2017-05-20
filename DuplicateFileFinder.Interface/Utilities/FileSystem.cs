using DuplicateFileFinder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFileFinder.Utilities
{
    public class FileSystem
    {
        public static Dictionary<string, List<FileSystemEntity>> AllFiles { get; set; }

        // Recursive method scanning for files in folders
        public static void TraverseDirectories(string directoryPath, HashSet<int> methods)
        {
            var files = GetFiles(directoryPath, methods);
            var directories = GetDirs(directoryPath);

            foreach (var file in files)
            {
                if (file != null)
                {
                    if (!AllFiles.ContainsKey(file.Hash))
                    {
                        AllFiles.Add(file.Hash, new List<FileSystemEntity>());
                    }

                    AllFiles[file.Hash].Add(file);
                }
            }

            foreach (var directory in directories)
            {
                TraverseDirectories(directory, methods);
            }
        }

        private static List<FileSystemEntity> GetFiles(string directoryPath, HashSet<int> methods)
        {
            List<FileSystemEntity> filesFound = new List<FileSystemEntity>();
            string[] files;

            try
            {
                files = Directory.GetFiles(directoryPath);
                foreach (var fileFound in files)
                {
                    FileInfo fileInfo = new FileInfo(fileFound);
                    string fileName = fileFound.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    var hash = HashGenerator.FileHash(fileName, directoryPath, methods);
                    long size = fileInfo.Length;
                    var file = new FileSystemEntity(fileName, directoryPath, hash, size);

                    filesFound.Add(file);
                }
            }
            catch (Exception e)
            {
                //ToDo: Implement some sort of log to tell the user that not all files were checked!
            }

            return filesFound;
        }

        private static List<string> GetDirs(string directoryPath)
        {
            List<string> dirsFound = new List<string>();
            string[] dirs;

            try
            {
                dirs = Directory.GetDirectories(directoryPath);
                foreach (var dirFound in dirs)
                {
                    if (!dirsFound.Contains(dirFound))
                    {
                        dirsFound.Add(dirFound);
                    }
                }
            }
            catch (Exception e)
            {
                dirs = new string[0];
                //ToDo: implement some sort of log to tell the user that not all folders were checked!
            }

            return dirsFound;
        }
    }
}
