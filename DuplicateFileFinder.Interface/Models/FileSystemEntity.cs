using System;
using System.Collections.Generic;
using System.IO;

namespace DuplicateFileFinder.Models
{
    public class FileSystemEntity
    {
        public string Name { get; private set; }
        public string Hash { get; set; }
        public string Path { get; private set; }
        public long Size { get; set; }

        public FileSystemEntity(string name, string path)
        {
            this.Name = name;
            this.Path = path;
            this.Hash = string.Empty;
            this.Size = 0;
        }

        // Delete entity
        [STAThread]
        public KeyValuePair<bool, string> FileDelete()
        {
            string message = String.Empty;
            bool state = false;

            if (File.Exists($@"{this.Path}\{this.Name}"))
            {
                try
                {
                    File.Delete($@"{this.Path}\{this.Name}");
                    state = true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    state = false;
                }
            }
            else
            {
                state = false;
                message = $@"The file specified does not exists:\n {this.Path}\{this.Name}";
            }

            var result = new KeyValuePair<bool, string>(state, message);
            return result;
        }
    }
}
