using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace YKWrandomizer.Tools
{
    public class ResourceReader
    {
        public string Path;

        public ResourceReader()
        {

        }

        public ResourceReader(string file)
        {
            this.Path = file;
        }

        public Stream GetResourceStream()
        {
            string resourcePath = Path;
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            List<string> source = new List<string>(executingAssembly.GetManifestResourceNames());
            resourcePath = source.FirstOrDefault((string r) => r.Contains(resourcePath));

            if (resourcePath == null)
            {
                throw new FileNotFoundException("Resource not found");
            }

            return executingAssembly.GetManifestResourceStream(resourcePath);
        }
    }
}

