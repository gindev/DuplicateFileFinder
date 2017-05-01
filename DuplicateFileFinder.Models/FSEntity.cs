using System.Collections.Generic;

namespace DuplicateFileFinder.Models
{
    public class FSEntity
    {
        public string Name { get; set; }

        public string Hash { get; set; }

        public bool IsFolder { get; set; }

        public ICollection<FSEntity> Children { get; set; }

        public FSEntity Parent { get; set; }

        public FSEntity()
        {
            this.IsFolder = false;
            this.Children = new HashSet<FSEntity>();
            this.Parent = null;
        }

        public FSEntity(string name, string hash, bool isFolder, ICollection<FSEntity> children, FSEntity parent)
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
