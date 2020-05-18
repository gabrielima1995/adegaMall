using Hangfire.Common;
using Hangfire.States;
using System;
using System.Linq.Expressions;

namespace Hangfire
{
    /// <summary>
    /// Helper responsible for basic functions for jobs.
    /// </summary>
    public static class JobHelper
    {
        /// <summary>
        /// Creates job for an action by placing it in the default Hangfire queue.
        /// </summary>
        /// <param name="methodCall">Action to create the job.</param>
        /// <returns></returns>
        public static string Enqueue(this Expression<Action> methodCall)
        {
            return Enqueue(methodCall, EnqueuedState.DefaultQueue);
        }

        /// <summary>
        /// Creates job for an action by placing in a Hangfire queue.
        /// </summary>
        /// <param name="methodCall">Action to create the job.</param>
        /// <param name="queueName">Name of queue.</param>
        /// <returns></returns>
        public static string Enqueue(this Expression<Action> methodCall, string queueName)
        {
            var client = new BackgroundJobClient();

            var state = new EnqueuedState(queueName);

            return client.Create(methodCall, state);
        }

        /// <summary>
        /// Creates recurring job for an action.
        /// </summary>
        /// <param name="methodCall">Action to create the job.</param>
        /// <param name="recurringJobId">Identification for the job.</param>
        /// <param name="cronExpression">Cron expression</param>
        public static void CreateRecurring(this Expression<Action> methodCall, string recurringJobId, string cronExpression)
        {
            var manager = new RecurringJobManager();

            manager.RemoveIfExists(recurringJobId);

            manager.AddOrUpdate(recurringJobId, Job.FromExpression(methodCall), cronExpression);
        }

        /// <summary>
        /// Creates a new background job based on a specified lambda expression and schedules it to be enqueued after a given delay.
        /// </summary>
        /// <param name="methodCall">Action to create the job.</param>
        /// <param name="delay">Delay, after which the job will be enqueued.</param>
        /// <returns></returns>
        public static string Schedule(this Expression<Action> methodCall, TimeSpan delay)
        {
            var client = new BackgroundJobClient();

            return client.Schedule(methodCall, delay);
        }

        /// <summary>
        /// Creates job for an action by placing it in the default Hangfire queue.
        /// </summary>
        /// <typeparam name="T">Type of class that contains the action to create the job.</typeparam>
        /// <param name="methodCall">Action to create the job.</param>
        /// <returns></returns>
        public static string Enqueue<T>(this Expression<Action<T>> methodCall)
        {
            return Enqueue<T>(methodCall, EnqueuedState.DefaultQueue);
        }

        /// <summary>
        /// Creates job for an action by placing in a Hangfire queue.
        /// </summary>
        /// <typeparam name="T">Type of class that contains the action to create the job.</typeparam>
        /// <param name="methodCall">Action to create the job.</param>
        /// <param name="queueName">Name of queue.</param>
        /// <returns></returns>
        public static string Enqueue<T>(this Expression<Action<T>> methodCall, string queueName)
        {
            var client = new BackgroundJobClient();

            var state = new EnqueuedState(queueName);

            return client.Create<T>(methodCall, state);
        }

        /// <summary>
        /// Creates a new background job based on a specified lambda expression and schedules it to be enqueued after a given delay.
        /// </summary>
        /// <param name="methodCall">Action to create the job.</param>
        /// <param name="delay">Delay, after which the job will be enqueued.</param>
        /// <returns></returns>
        public static string Schedule<T>(this Expression<Action<T>> methodCall, TimeSpan delay)
        {
            var client = new BackgroundJobClient();

            return client.Schedule<T>(methodCall, delay);
        }

        /// <summary>
        /// Creates recurring job for an action.
        /// </summary>
        /// <typeparam name="T">Type of class that contains the action to create the job.</typeparam>
        /// <param name="methodCall">Action to create the job.</param>
        /// <param name="recurringJobId">Identification for the job.</param>
        /// <param name="cronExpression">Cron expression</param>
        public static void CreateRecurring<T>(this Expression<Action<T>> methodCall, string recurringJobId, string cronExpression)
        {
            var manager = new RecurringJobManager();

            manager.RemoveIfExists(recurringJobId);

            manager.AddOrUpdate(recurringJobId, Job.FromExpression<T>(methodCall), cronExpression);
        }
    }
}