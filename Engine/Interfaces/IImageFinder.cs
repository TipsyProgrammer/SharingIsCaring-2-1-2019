using System.Collections.Generic;

namespace Engine.Interfaces
{
    public interface IImageFinder
    {
        IEnumerable<string> LocateImageNamesFromIndexFile(string indexFilePath);
    }
}