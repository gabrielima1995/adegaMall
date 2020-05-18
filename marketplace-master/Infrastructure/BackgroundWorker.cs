using Domain.Interfaces;
using Hangfire;
using System;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class BackgroundWorker : IBackgroundWorker
    {
        public void CreateRecurring(Expression<Action> methodCall, string recurringJobId, string cronExpression)
        {
            JobHelper.CreateRecurring(methodCall, recurringJobId, cronExpression);
        }

        public void CreateRecurring<T>(Expression<Action<T>> methodCall, string recurringJobId, string cronExpression)
        {
            JobHelper.CreateRecurring(methodCall, recurringJobId, cronExpression);
        }

        public string Enqueue(Expression<Action> methodCall)
        {
            return JobHelper.Enqueue(methodCall);
        }

        public string Enqueue(Expression<Action> methodCall, string queueName)
        {
            return JobHelper.Enqueue(methodCall, queueName);
        }

        public string Enqueue<T>(Expression<Action<T>> methodCall)
        {
            return JobHelper.Enqueue(methodCall);
        }

        public string Enqueue<T>(Expression<Action<T>> methodCall, string queueName)
        {
            return JobHelper.Enqueue(methodCall, queueName);
        }

        public string Schedule(Expression<Action> methodCall, TimeSpan delay)
        {
            return JobHelper.Schedule(methodCall, delay);
        }

        public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        {
            return JobHelper.Schedule(methodCall, delay);
        }
    }
}