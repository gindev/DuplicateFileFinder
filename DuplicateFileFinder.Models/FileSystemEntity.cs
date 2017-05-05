using System.Collections.Generic;

namespace DuplicateFileFinder.Models
{
    public class FileSystemEntity
    {
        public string Name { get; set; }

        public string Hash { get; set; }

        public bool IsFolder { get; set; }

        public ICollection<FileSystemEntity> Children { get; set; }

        public FileSystemEntity Parent { get; set; }

        public FileSystemEntity()
        {
            this.IsFolder = false;
            this.Children = new HashSet<FileSystemEntity>();
            this.Parent = null;
        }

        public FileSystemEntity(string name, string hash, bool isFolder, ICollection<FileSystemEntity> children, FileSystemEntity parent)
            : this()
        {
            this.Name = name;
            this.Hash = hash;

            if (isFolder)
            {
                this.IsFolder = isFolder;
            }

            if(children != null)
            {
                this.Children = children;
            }

            if(parent != null)
            {
                this.Parent = parent;
            }
        }
    }
}
