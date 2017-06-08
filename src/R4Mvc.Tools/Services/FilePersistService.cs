using Microsoft.CodeAnalysis;
using System.IO;

namespace R4Mvc.Tools.Services
{
    public class FilePersistService : IFilePersistService
    {
        public void WriteFile(SyntaxNode fileTree, string filePath)
        {
            using (var textWriter = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                fileTree.WriteTo(textWriter);
            }
        }
    }
}
