/*
 * Based on https://github.com/vkhorikov/CSharpFunctionalExtensions/blob/master/CSharpFunctionalExtensions/Result.cs
 * And https://github.com/vkhorikov/CSharpFunctionalExtensions/blob/master/CSharpFunctionalExtensions/ResultExtensions.cs
 */

using System;

namespace Framework
{
    public static class OperationResultFunctionalExtensions
    {
        //public static OperationResult<K, E> OnSuccess<T, K, E>(this OperationResult<T, E> result,
        //    Func<T, K> func)
        //{
        //    if (result.IsFailure)
        //        return OperationResult.Fail<K, E>(result.Error);

        //    return OperationResult.Ok<K, E>(func(result.Value));
        //}

        //public static OperationResult<K> OnSuccess<T, K>(this OperationResult<T> result, Func<T, K> func)
        //{
        //    if (result.IsFailure)
        //        return OperationResult.Fail<K>(result.Error);

        //    return OperationResult.Ok(func(result.Value));
        //}

        public static OperationResult<T> OnSuccess<T>(this OperationResult result, Func<T> func)
        {
            if (result.IsFailure)
                return new OperationResult<T>(result.Errors);

            return new OperationResult<T>(func());
        }

        //public static OperationResult<K, E> OnSuccess<T, K, E>(this OperationResult<T, E> result,
        //    Func<T, OperationResult<K, E>> func)
        //{
        //    if (result.IsFailure)
        //        return OperationResult.Fail<K, E>(result.Error);

        //    return func(result.Value);
        //}

        public static OperationResult<K> OnSuccess<T, K>(this OperationResult<T> result, Func<T, OperationResult<K>> func)
        {
            if (result.IsFailure)
                return new OperationResult<K>(result.Errors);

            return func(result.Result);
        }

        public static OperationResult<T> OnSuccess<T>(this OperationResult result, Func<OperationResult<T>> func)
        {
            if (result.IsFailure)
                return new OperationResult<T>(result.Errors);

            return func();
        }

        //public static OperationResult<K, E> OnSuccess<T, K, E>(this OperationResult<T, E> result,
        //    Func<OperationResult<K, E>> func)
        //{
        //    if (result.IsFailure)
        //        return OperationResult.Fail<K, E>(result.Error);

        //    return func();
        //}

        public static OperationResult<K> OnSuccess<T, K>(this OperationResult<T> result, Func<OperationResult<K>> func)
        {
            if (result.IsFailure)
                return new OperationResult<K>(result.Errors);

            return func();
        }

        //public static OperationResult<K> OnSuccess<T, K, E>(this OperationResult<T, E> result,
        //    Func<T, OperationResult<K>> func)
        //{
        //    if (result.IsFailure)
        //        return OperationResult.Fail<K, E>(result.Error);

        //    return func(result.Value);
        //}

        //public static OperationResult OnSuccess<T, K, E>(this OperationResult<T, E> result,
        //    Func<T, OperationResult> func)
        //{
        //    if (result.IsFailure)
        //        return OperationResult.Fail<K, E>(result.Error);

        //    return func(result.Value);
        //}

        public static OperationResult OnSuccess<T>(this OperationResult<T> result, Func<T, OperationResult> func)
        {
            if (result.IsFailure)
                return new OperationResult(result.Errors);

            return func(result.Result);
        }

        public static OperationResult OnSuccess(this OperationResult result, Func<OperationResult> func)
        {
            if (result.IsFailure)
                return result;

            return func();
        }

        //public static OperationResult<T, E> Ensure<T, E>(this OperationResult<T, E> result,
        //    Func<T, bool> predicate, E error)
        //{
        //    if (result.IsFailure)
        //        return result;

        //    if (!predicate(result.Value))
        //        return OperationResult.Fail<T, E>(error);

        //    return result;
        //}

        public static OperationResult<T> Ensure<T>(this OperationResult<T> result, Func<T, bool> predicate, string errorMessage)
        {
            if (result.IsFailure)
                return result;

            if (!predicate(result.Result))
                return new OperationResult<T>("", errorMessage);

            return result;
        }

        public static OperationResult Ensure(this OperationResult result, Func<bool> predicate, string errorMessage)
        {
            if (result.IsFailure)
                return result;

            if (!predicate())
                return OperationResult.Fail(errorMessage);

            return result;
        }

        //public static OperationResult<K, E> Map<T, K, E>(this OperationResult<T, E> result,
        //    Func<T, K> func)
        //    => result.OnSuccess(func);

        //public static OperationResult<K> Map<T, K>(this OperationResult<T> result, Func<T, K> func)
        //    => result.OnSuccess(func);

        //public static OperationResult<T> Map<T>(this OperationResult result, Func<T> func)
        //    => result.OnSuccess(func);

        //public static OperationResult<T, E> OnSuccess<T, E>(this OperationResult<T, E> result,
        //    Action<T> action)
        //{
        //    if (result.IsSuccess)
        //    {
        //        action(result.Value);
        //    }

        //    return result;
        //}

        public static OperationResult<T> OnSuccess<T>(this OperationResult<T> result, Action<T> action)
        {
            if (result.OK)
                action(result.Result);

            return result;
        }

        public static OperationResult OnSuccess(this OperationResult result, Action action)
        {
            if (result.OK)
                action();

            return result;
        }

        //public static OperationResult OnSuccessTry(this OperationResult result, Action action, Func<Exception, string> errorHandler = null)
        //{
        //    return result.IsFailure
        //        ? result
        //        : OperationResult.Try(action, errorHandler);
        //}

        //public static OperationResult<T> OnSuccessTry<T>(this OperationResult result, Func<T> func, Func<Exception, string> errorHandler = null)
        //{
        //    return result.IsFailure
        //        ? OperationResult.Fail<T>(result.Error)
        //        : OperationResult.Try(func, errorHandler);
        //}

        //public static OperationResult OnSuccessTry<T>(this OperationResult<T> result, Action<T> action, Func<Exception, string> errorHandler = null)
        //{
        //    return result.IsFailure
        //        ? OperationResult.Fail(result.Error)
        //        : OperationResult.Try(() => action(result.Value), errorHandler);
        //}

        //public static OperationResult<K> OnSuccessTry<T, K>(this OperationResult<T> result, Func<T, K> func, Func<Exception, string> errorHandler = null)
        //{
        //    return result.IsFailure
        //        ? OperationResult.Fail<K>(result.Error)
        //        : OperationResult.Try(() => func(result.Value), errorHandler);
        //}

        public static T OnBoth<T>(this OperationResult result, Func<OperationResult, T> func)
        {
            return func(result);
        }

        public static K OnBoth<T, K>(this OperationResult<T> result, Func<OperationResult<T>, K> func)
        {
            return func(result);
        }

        //public static K OnBoth<T, K, E>(this OperationResult<T, E> result, Func<OperationResult<T, E>, K> func)
        //{
        //    return func(result);
        //}

        //public static T OnBoth<T, E>(this OperationResult<T, E> result,
        //    Func<OperationResult<T, E>, T> func)
        //{
        //    return func(result);
        //}

        //public static OperationResult<T, E> OnFailure<T, E>(this OperationResult<T, E> result,
        //    Action action)
        //{
        //    if (result.IsFailure)
        //    {
        //        action();
        //    }

        //    return result;
        //}

        public static OperationResult<T> OnFailure<T>(this OperationResult<T> result, Action action)
        {
            if (result.IsFailure)
                action();

            return result;
        }

        public static OperationResult OnFailure(this OperationResult result, Action action)
        {
            if (result.IsFailure)
            {
                action();
            }

            return result;
        }

        //public static OperationResult<T, E> OnFailure<T, E>(this OperationResult<T, E> result,
        //    Action<E> action)
        //{
        //    if (result.IsFailure)
        //    {
        //        action(result.Error);
        //    }

        //    return result;
        //}

        public static OperationResult<T> OnFailure<T>(this OperationResult<T> result, Action<ErrorDictionary> action)
        {
            if (result.IsFailure)
                action(result.Errors);

            return result;
        }

        public static OperationResult OnFailure(this OperationResult result, Action<ErrorDictionary> action)
        {
            if (result.IsFailure)
                action(result.Errors);

            return result;
        }

        //public static OperationResult<T, E> OnFailureCompensate<T, E>(this OperationResult<T, E> result,
        //    Func<OperationResult<T, E>> func)
        //{
        //    if (result.IsFailure)
        //        return func();

        //    return result;
        //}

        public static OperationResult<T> OnFailureCompensate<T>(this OperationResult<T> result, Func<OperationResult<T>> func)
        {
            if (result.IsFailure)
                return func();

            return result;
        }

        public static OperationResult OnFailureCompensate(this OperationResult result, Func<OperationResult> func)
        {
            if (result.IsFailure)
                return func();

            return result;
        }

        //public static OperationResult<T, E> OnFailureCompensate<T, E>(this OperationResult<T, E> result,
        //    Func<E, OperationResult<T, E>> func)
        //{
        //    if (result.IsFailure)
        //        return func(result.Error);

        //    return result;
        //}

        public static OperationResult<T> OnFailureCompensate<T>(this OperationResult<T> result, Func<ErrorDictionary, OperationResult<T>> func)
        {
            if (result.IsFailure)
                return func(result.Errors);

            return result;
        }

        public static OperationResult OnFailureCompensate(this OperationResult result, Func<ErrorDictionary, OperationResult> func)
        {
            if (result.IsFailure)
                return func(result.Errors);

            return result;
        }

        //public static OperationResult Combine(this IEnumerable<OperationResult> results, string errorMessagesSeparator)
        //{
        //    return OperationResult.Combine(errorMessagesSeparator, results as OperationResult[] ?? results.ToArray());
        //}

        //public static OperationResult Combine(this IEnumerable<OperationResult> results)
        //{
        //    return OperationResult.Combine(results as OperationResult[] ?? results.ToArray());
        //}

        //public static OperationResult<IEnumerable<T>> Combine<T>(this IEnumerable<OperationResult<T>> results, string errorMessagesSeparator)
        //{
        //    var data = results as OperationResult<T>[] ?? results.ToArray();

        //    var result = OperationResult.Combine(errorMessagesSeparator, data);

        //    return result.IsSuccess
        //        ? new OperationResult<IEnumerable<T>>(data.Select(e => e.Result))
        //        : new OperationResult<IEnumerable<T>>(result.Errors);
        //}

        //public static OperationResult<K> Combine<T, K>(this IEnumerable<OperationResult<T>> results,
        //    Func<IEnumerable<T>, K> composer,
        //    string errorMessageSeparator)
        //{
        //    OperationResult<IEnumerable<T>> result = results.Combine(errorMessageSeparator);

        //    return result.OK
        //        ? new OperationResult<K>(composer(result.Result))
        //        : new OperationResult<K>(result.Errors);
        //}
    }
}