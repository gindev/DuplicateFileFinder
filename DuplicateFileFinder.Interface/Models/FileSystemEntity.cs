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
    }
}
