using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace NewContext.Application.Startup
{
    public interface IStartup
    {
        Task<Result> RunAsync();
    }
}
