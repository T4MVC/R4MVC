using System.IO;
using System.Threading.Tasks;

namespace R4Mvc.Tools.Services
{
    public interface IGeneratedFileTesterService
    {
        Task<bool> IsGenerated(Stream fileStream);
    }
}
