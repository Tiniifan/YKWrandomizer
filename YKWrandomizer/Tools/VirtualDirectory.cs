using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace YKWrandomizer.Tools
{
    public class VirtualDirectory
    {
        public string Name;

        public List<VirtualDirectory> Folders;

        public Dictionary<string, SubMemoryStream> Files;

        public Color Color = Color.Black;

        public VirtualDirectory()
        {
            Folders = new List<VirtualDirectory>();
            Files = new Dictionary<string, SubMemoryStream>();
        }

        public VirtualDirectory(string name)
        {
            Name = name;
            Folders = new List<VirtualDirectory>();
            Files = new Dictionary<string, SubMemoryStream>();
        }

        public VirtualDirectory GetFolder(string name)
        {
            return Folders.FirstOrDefault(folder => folder.Name == name);
        }

        public VirtualDirectory GetFolderFromFullPath(string path)
        {
            var pathSplit = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var current = this;

            // Get Path
            for (int i = 0; i < pathSplit.Length; i++)
            {
                current = current.GetFolder(pathSplit[i]);

                if (current == null)
                {
                    throw new DirectoryNotFoundException(path + " not exist");
                }
            }

            return current;
        }

        public bool IsFolderExists(string path)
        {
            var pathSplit = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var current = this;

            // Get Path
            for (int i = 0; i < pathSplit.Length; i++)
            {
                current = current.GetFolder(pathSplit[i]);

                if (current == null)
                {
                    return false;
                }
            }

            return true;
        }

        public List<VirtualDirectory> GetAllFolders()
        {
            List<VirtualDirectory> allFolders = new List<VirtualDirectory>();

            foreach (VirtualDirectory folder in Folders)
            {
                allFolders.Add(folder);
            }

            return allFolders;
        }

        public Dictionary<string, VirtualDirectory> GetAllFoldersAsDictionnary()
        {
            var directories = new Dictionary<string, VirtualDirectory> { { Name + "/", this } };
            foreach (var folder in Folders)
            {
                foreach (var subDirectory in folder.GetAllFoldersAsDictionnary())
                {
                    var key = Name + "/" + subDirectory.Key;
                    if (!directories.ContainsKey(key))
                    {
                        directories.Add(key, subDirectory.Value);
                    }
                }
            }

            return directories;
        }

        public byte[] GetFileFromFullPath(string path)
        {
            var pathSplit = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var fileName = pathSplit[pathSplit.Length - 1];
            var current = this;

            // Get Path
            for (int i = 0; i < pathSplit.Length - 1; i++)
            {
                current = current.GetFolder(pathSplit[i]);

                if (current == null)
                {
                    throw new DirectoryNotFoundException(path + " not exist");
                }
            }

            if (current.Files.ContainsKey(fileName))
            {
                SubMemoryStream subMemoryStream = current.Files[fileName];

                // Fill subMemoryStream
                if (subMemoryStream.ByteContent == null)
                {
                    subMemoryStream.Read();
                }

                return subMemoryStream.ByteContent;
            }
            else
            {
                throw new FileNotFoundException(fileName + " not exist");
            }
        }

        public Dictionary<string, SubMemoryStream> GetAllFiles()
        {
            Dictionary<string, SubMemoryStream> allFiles = new Dictionary<string, SubMemoryStream>();

            foreach (KeyValuePair<string, SubMemoryStream> file in Files)
            {
                allFiles.Add(Name + "/" + file.Key, file.Value);
            }

            foreach (VirtualDirectory folder in Folders)
            {
                Dictionary<string, SubMemoryStream> subFiles = folder.GetAllFiles();
                foreach (KeyValuePair<string, SubMemoryStream> file in subFiles)
                {
                    allFiles.Add(Name + "/" + file.Key, file.Value);
                }
            }

            return allFiles;
        }

        public void AddFile(string name, SubMemoryStream data)
        {
            Files.Add(name, data);
        }

        public void AddFolder(string name)
        {
            Folders.Add(new VirtualDirectory(name));
        }

        public void AddFolder(VirtualDirectory folder)
        {
            Folders.Add(folder);
        }

        public long GetSize()
        {
            long size = 0;

            foreach (SubMemoryStream file in Files.Values)
            {
                if (file.ByteContent == null)
                {
                    size += file.Size;
                }
                else
                {
                    size += file.ByteContent.Length;
                }
            }

            foreach (VirtualDirectory folder in Folders)
            {
                size += folder.GetSize();
            }

            return size;
        }


        public void Reorganize()
        {
            // Retrieve all folders and order them by name
            VirtualDirectory[] folders = GetAllFolders().OrderBy(x => x.Name).ToArray();

            // Iterate through each folder path
            foreach (var folderPath in folders)
            {
                // Split the path into individual folder names
                var pathSplit = folderPath.Name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var currentPath = "";

                // Iterate through each folder name in the path
                foreach (var folderName in pathSplit)
                {
                    // Build the current path
                    currentPath = Path.Combine(currentPath, folderName) + "\\";

                    // Check if the folder doesn't exist and has a valid depth
                    if (currentPath.Count(c => c == '\\') > 1 && GetFolder(currentPath.Replace("\\", "/")) == null)
                    {
                        // Add the folder to the virtual directory
                        AddFolder(currentPath.Replace("\\", "/"));
                    }
                }
            }

            // Reorganize folders with multiple levels
            folders = GetAllFolders().OrderBy(x => x.Name).ToArray();
            var result = new VirtualDirectory("");
            result.Files = Files;

            // Iterate through each folder in ordered folders
            foreach (var folder in folders.Where(x => x.Name != ""))
            {
                // Split the folder name into individual parts
                var path = folder.Name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var current = result;

                // Traverse through the folder structure
                for (int i = 0; i < path.Length; i++)
                {
                    // Check if the folder doesn't exist
                    if (current.GetFolder(path[i]) == null)
                    {
                        // Create a new folder and assign files
                        VirtualDirectory newFolder = new VirtualDirectory(path[i]);
                        newFolder.Files = folder.Files;
                        current.AddFolder(newFolder);
                    }

                    // Move to the next level in the virtual directory
                    current = current.GetFolder(path[i]);
                }
            }

            // Update the files and folders in the current virtual directory
            Files = result.Files;
            Folders = result.Folders;
        }

        public void SortAlphabetically()
        {
            Folders.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (VirtualDirectory folder in Folders)
            {
                folder.SortAlphabetically();
            }

            var sortedFiles = Files.OrderBy(file => file.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
            Files = sortedFiles;
        }

        public void Print()
        {
            Print(this);
        }

        public void Print(VirtualDirectory directory, int level = 0)
        {
            string indentation = new string('\t', level);
            Console.WriteLine($"{indentation}/{directory.Name}: ");

            foreach (VirtualDirectory subDirectory in directory.Folders)
            {
                Print(subDirectory, level + 1);
            }

            foreach (KeyValuePair<string, SubMemoryStream> files in directory.Files)
            {
                indentation = new string('\t', level + 1);
                Console.WriteLine($"{indentation}{files.Key}");
            }
        }
    }
}

