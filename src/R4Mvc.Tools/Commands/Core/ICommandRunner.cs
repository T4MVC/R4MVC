using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace R4Mvc.Tools.Commands.Core
{
    public interface ICommandRunner
    {
        Task Run(string projectPath, IConfiguration configuration, string[] args);
    }
}
