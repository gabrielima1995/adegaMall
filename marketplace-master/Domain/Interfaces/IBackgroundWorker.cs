using System;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IBackgroundWorker
    {
        string Enqueue(Expression<Action> methodCall);

        string Enqueue(Expression<Action> methodCall, string queueName);

        string Enqueue<T>(Expression<Action<T>> methodCall);

        string Enqueue<T>(Expression<Action<T>> methodCall, string queueName);

        void CreateRecurring(Expression<Action> methodCall, string recurringJobId, string cronExpression);

        void CreateRecurring<T>(Expression<Action<T>> methodCall, string recurringJobId, string cronExpression);

        string Schedule(Expression<Action> methodCall, TimeSpan delay);

        string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
    }
}
