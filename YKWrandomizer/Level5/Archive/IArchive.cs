using System.Windows.Forms;
using YKWrandomizer.Tools;

namespace YKWrandomizer.Level5.Archive
{
    public interface IArchive
    {
        string Name { get; }

        VirtualDirectory Directory { get; set; }

        void Save(string path, ProgressBar progressBar);

        void Close();
    }
}
