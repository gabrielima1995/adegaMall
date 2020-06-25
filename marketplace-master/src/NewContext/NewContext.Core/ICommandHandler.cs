using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace NewContext.Core
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task<Result> HandleAsync(TCommand command);
    }
}
