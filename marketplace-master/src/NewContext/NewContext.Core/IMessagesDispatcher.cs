using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace NewContext.Core
{
    public interface IMessagesDispatcher
    {
        Task<Result> CommandAsync(ICommand command);

        Task PublishAsync(IDomainEvent domainEvent);

        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
    }
}
