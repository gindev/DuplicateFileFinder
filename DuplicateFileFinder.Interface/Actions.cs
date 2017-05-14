using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DuplicateFileFinder.Models;
using System.Linq;

namespace DuplicateFileFinder.Interface
{
    public static class Actions
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

        private static List<FileSystemEntity> GetFiles(string directoryPath)
        {
            List<FileSystemEntity> filesFound = new List<FileSystemEntity>();

            foreach (var fileFound in Directory.GetFiles(directoryPath))
            {
                var file = new FileSystemEntity(fileFound.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).Last(),directoryPath);
                filesFound.Add(file);
            }

            return filesFound;
        }

        private static List<string> GetDirs(string directoryPath)
        {
            List<string> dirsFound = new List<string>();

            foreach (var dirFound in Directory.GetDirectories(directoryPath))
            {
                if (!dirsFound.Contains(dirFound))
                {
                    dirsFound.Add(dirFound);
                }
                continue;
            }

            return dirsFound;
        }

        public static void TraverseDirectories(string directoryPath, Dictionary<string, List<FileSystemEntity>> foundFiles)
        {
            foreach (var fileSystemEntity in GetFiles(directoryPath))
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
                TraverseDirectories(subDirEntity,foundFiles);
            }
        }
    }
}
