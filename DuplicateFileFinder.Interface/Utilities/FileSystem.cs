using DuplicateFileFinder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DuplicateFileFinder.Utilities
{
    public class FileSystem
    {
        // Recursive method scanning for files in folders
        [STAThread]
        public static Dictionary<string, List<FileSystemEntity>> TraverseDirectories(string directoryPath, HashSet<int> methods)
        {
            var hashedFiles = new Dictionary<string, List<FileSystemEntity>>();
            
            List<FileSystemEntity> files = GetFiles(directoryPath, methods);
            List<string> directories = GetDirs(directoryPath);

            foreach (var directory in directories)
            {
                var list = TraverseDirectories(directory, methods);
                foreach (var item in list)
                {
                    if (!hashedFiles.ContainsKey(item.Key))
                    {
                        hashedFiles.Add(item.Key, item.Value);
                    }
                    else
                    {
                        hashedFiles[item.Key].AddRange(item.Value);
                    }
                }
            }

            foreach (var file in files)
            {
                if (file != null)
                {
                    if (!hashedFiles.ContainsKey(file.Hash))
                    {
                        hashedFiles.Add(file.Hash, new List<FileSystemEntity>());
                    }

                    hashedFiles[file.Hash].Add(file);
                }
            }
            
            return hashedFiles;
        }

        [STAThread]
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
                    var file = new FileSystemEntity(fileName, directoryPath);
                    file.Size = fileInfo.Length;
                    file.Hash = HashGenerator.FileHash(file, methods);
                    filesFound.Add(file);
                }
            }
            catch (Exception e)
            {
                //ToDo: Implement some sort of log to tell the user that not all files were checked!
            }

            return filesFound;
        }

        [STAThread]
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
                //ToDo: implement some sort of log to tell the user that not all folders were checked!
            }

            return dirsFound;
        }

        public static KeyValuePair<bool, string> FileDelete(string fileWithPath)
        {
            string message = "ok";
            bool state = false;
            KeyValuePair<bool, string> result = new KeyValuePair<bool, string>(false, message);

            if (File.Exists($@"{fileWithPath}"))
            {
                try
                {
                    File.Delete($@"{fileWithPath}");
                    state = true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    state = false;
                }
                finally
                {
                    result = new KeyValuePair<bool, string>(state, message);
                }
            }
            else
            {
                MessageBox.Show($@"The file specified does not exists:\n {fileWithPath}");
                
            }
            return result;
        }
    }
}
