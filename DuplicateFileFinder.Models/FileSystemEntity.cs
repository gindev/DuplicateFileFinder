using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DuplicateFileFinder.Models
{
    public class FileSystemEntity
    {
        public string Name { get; private set; }

        public string Hash { get; private set; }

        public string Path { get; private set; }

        public FileSystemEntity(string name, string path)
        {
            this.Name = name;

            this.Path = path;

            this.Hash = this.ComputeHash($"{this.Path}\\{this.Name}");
        }

        private string ComputeHash(string fileName)
        {
            byte[] tempHash;
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(fileName))
            {
                tempHash = md5.ComputeHash(stream);
            }
            return tempHash.ToString();
        }
    }
}
