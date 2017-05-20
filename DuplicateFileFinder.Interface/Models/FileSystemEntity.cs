namespace DuplicateFileFinder.Models
{
    public class FileSystemEntity
    {
        public string Name { get; private set; }

        public string Hash { get; private set; }

        public string Path { get; private set; }

        public long Size { get; private set; }

        public FileSystemEntity(string name, string path, string hash, long size)
        {
            this.Name = name;

            this.Path = path;

            this.Hash = hash;

            this.Size = size;
        }
    }
}
