using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace R4Mvc.Tools.Commands
{
    public interface ICommand
    {
        Task Run(string projectPath, IConfiguration configuration);
    }
}
