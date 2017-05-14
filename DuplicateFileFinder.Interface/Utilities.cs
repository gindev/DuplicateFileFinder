using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DuplicateFileFinder.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DuplicateFileFinder.Interface
{
    public static class Utilities
    {
        public static string SelectFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select forlder to scan for duplicates";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fbd.SelectedPath;
                return sSelectedPath;
            }

            return null;
        }

        public static void ResetControls(Control.ControlCollection controlsCollection)
        {
            foreach (var control in controlsCollection)
            {
                (control as CheckBox).Checked = false;
            }
        }

        #region TraverseDirectories
        /// <summary>
        /// Traverse directories and subdirectories and list all files within.
        /// </summary>
        /// <param name="directoryPath">The current working directory</param>
        /// <param name="foundFiles">Reference to a dictionary, containing all found files.</param>
        /// <param name="method">File comparison method.</param>
        public static void TraverseDirectories(string directoryPath, Dictionary<string, List<FileSystemEntity>> foundFiles, int method)
        {
            var files = GetFiles(directoryPath, method);

            foreach (var fileSystemEntity in files)
            {
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
                TraverseDirectories(subDirEntity, foundFiles, method);
            }
        }

        private static List<FileSystemEntity> GetFiles(string directoryPath, int method)
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

            foreach (var fileFound in files)
            {
                string fileName = fileFound.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                var hash = FileHash(fileName, directoryPath, method);
                uint size = 0;
                var file = new FileSystemEntity(fileName, directoryPath, hash, size);

                filesFound.Add(file);
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

        #region HashGenerators
        private static string FileHash(string fileName, string filePath, int method)
        {
            string hash = string.Empty;
            if (method == 1)
            {
                hash = CalculateMD5Hash(fileName);
            }
            else if (method == 2)
            {
                using (FileStream file = new FileStream($"{filePath}\\{fileName}", FileMode.Open))
                {
                    hash = CalculateMD5Hash(file);
                }
            }

            return hash;
        }

        private static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.Default.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        private static string CalculateMD5Hash(FileStream input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(input);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
        #endregion

        public static void DisableControls(HashSet<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Enabled = false;
            }
        }

        public static void EnableControls(HashSet<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Enabled = true;
            }
        }
    }
}
