using WPLGSS.ViewModels;

namespace WPLGSS.Services
{
    public class SequenceFile
    {
        public SequenceFile(string path, SequenceViewModel sequence)
        {
            Path = path;
            Sequence = sequence;
        }

        public string Path { get; }
        public SequenceViewModel Sequence { get; }
    }
}