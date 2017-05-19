using DuplicateFileFinder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DuplicateFileFinder.Utilities
{
    class FileSystem
    {
        #region TraverseDirectories
        /// <summary>
        /// Traverse directories and subdirectories and list all files within.
        /// </summary>
        /// <param name="directoryPath">The current working directory</param>
        /// <param name="foundFiles">Reference to a dictionary, containing all found files.</param>
        /// <param name="methods">File comparison method.</param>
        public static void TraverseDirectories(string directoryPath, Dictionary<string, List<FileSystemEntity>> foundFiles, HashSet<int> methods)
        {
            var files = GetFiles(directoryPath, methods);

            foreach (var fileSystemEntity in files)
            {
                if(fileSystemEntity == null)
                {
                    continue;
                }

                if (!foundFiles.ContainsKey(fileSystemEntity.Hash))
                {
                    var list = new List<FileSystemEntity>();
                    list.Add(fileSystemEntity);
                    foundFiles.Add(fileSystemEntity.Hash, list);
                    continue;
                }

                foundFiles[fileSystemEntity.Hash].Add(fileSystemEntity);
            }

            foreach (var subDirEntity in GetDirs(directoryPath))
            {
                TraverseDirectories(subDirEntity, foundFiles, methods);
            }
        }

        private static List<FileSystemEntity> GetFiles(string directoryPath, HashSet<int> methods)
        {
            List<FileSystemEntity> filesFound = new List<FileSystemEntity>();

            string[] files;
            try
            {
                files = Directory.GetFiles(directoryPath);
            }
            catch (Exception e)
            {
                files = new string[0];
                //ToDo: Implement some sort of log to tell the user that not all files were checked!
            }

            Parallel.ForEach(files, fileFound =>
            {
                string fileName = fileFound.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                var hash = HashGenerator.FileHash(fileName, directoryPath, methods);
                uint size = 0;
                var file = new FileSystemEntity(fileName, directoryPath, hash, size);

                filesFound.Add(file);
            });
            //foreach (var fileFound in files)
            //{
            //    string fileName = fileFound.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).Last();
            //    var hash = HashGenerator.FileHash(fileName, directoryPath, methods);
            //    uint size = 0;
            //    var file = new FileSystemEntity(fileName, directoryPath, hash, size);
            //
            //    filesFound.Add(file);
            //}

            return filesFound;
        }

        private static List<string> GetDirs(string directoryPath)
        {
            List<string> dirsFound = new List<string>();

            string[] dirs;

            try
            {
                dirs = Directory.GetDirectories(directoryPath);
            }
            catch (Exception e)
            {
                dirs = new string[0];
                //ToDo: implement some sort of log to tell the user that not all folders were checked!
            }

            foreach (var dirFound in dirs)
            {
                if (!dirsFound.Contains(dirFound))
                {
                    dirsFound.Add(dirFound);
                }
                continue;
            }

            return dirsFound;
        }
        #endregion
    }
}
