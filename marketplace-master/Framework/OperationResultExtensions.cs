using System;

namespace Framework
{
    public static class OperationResultExtensions
    {
        /// <summary>
        /// Checks if operation result is valid and execute action.
        /// </summary>
        /// <param name="operation">Operation result data.</param>
        /// <param name="action">Action to perform if result is valid.</param>
        public static OperationResult Then(this OperationResult operation, Action action)
        {
            if (operation.OK)
            {
                action.Invoke();
            }

            return operation;
        }

        /// <summary>
        /// Checks if operation result is valid and execute action, with instance result by parameter.
        /// </summary>
        /// <param name="operation">Command result data.</param>
        /// <param name="func">Action to perform if result is valid.</param>
        public static OperationResult Then<T>(this OperationResult<T> operation, Func<T, OperationResult> func)
        {
            if (operation.OK)
            {
                return func.Invoke(operation.Result);
            }

            return operation;
        }

        /// <summary>
        /// Checks if operation result is valid and execute action.
        /// </summary>
        /// <param name="operation">Command result data.</param>
        /// <param name="func">Action to perform if result is valid.</param>
        public static OperationResult<T> Then<T>(this OperationResult operation, Func<OperationResult<T>> func)
        {
            if (operation.OK)
            {
                return func.Invoke();
            }

            return new OperationResult<T>(operation);
        }

        /// <summary>
        /// Checks if operation result is valid and execute action, with instance result by parameter.
        /// </summary>
        /// <param name="operation">Command result data.</param>
        /// <param name="func">Action to perform if result is valid.</param>
        public static OperationResult<T2> Then<T, T2>(this OperationResult<T> operation, Func<T, OperationResult<T2>> func)
        {
            if (operation.OK)
            {
                return func.Invoke(operation.Result);
            }

            return operation.MapTo(_ => default(T2));
        }

        /// <summary>
        /// Maps a operation to a new operation. If valid, returns with a new result. Otherwise, new result is null.
        /// </summary>
        /// <typeparam name="TEntity">Type of result of operation.</typeparam>
        /// <typeparam name="TMapped">type of result of new operation.</typeparam>
        /// <param name="result">Command result to perform.</param>
        /// <param name="map">Function that perform mapping to new result.</param>
        /// <returns>New operation with new result, if valid. Otherwise, new result is null.</returns>
        public static OperationResult<TMapped> MapTo<TEntity, TMapped>(this OperationResult<TEntity> result,
            Func<TEntity, TMapped> map)
        {
            var newResult = result.OK ? map.Invoke(result.Result) : default(TMapped);

            return new OperationResult<TMapped>(newResult, result.Errors);
        }
    }
}