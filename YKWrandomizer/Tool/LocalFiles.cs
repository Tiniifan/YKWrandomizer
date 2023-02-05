using System.IO;

namespace YKWrandomizer.Tool
{
    public class LocalFile
    {
        public string Path;

        public byte[] Data;

        public LocalFile(string path)
        {
            Path = path;
            Data = File.ReadAllBytes(path);
        }
    }
}