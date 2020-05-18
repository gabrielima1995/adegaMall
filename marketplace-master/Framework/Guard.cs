using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Framework
{
    public interface IGuardClause { }

    public class GuardEnsure : IGuardClause { }

    public static class Guard
    {
        public static GuardEnsure Ensure = new GuardEnsure();
    }

    public static class GuardEnsure_BooleanExtensions
    {
        public static void IsFalse(this IGuardClause g, bool input, string argumentName, string errorMessage = null)
        {
            if (input != false) throw new ArgumentException(errorMessage ?? "Must be false.", argumentName);
        }

        public static void IsTrue(this IGuardClause g, bool input, string argumentName, string errorMessage = null)
        {
            if (input != true) throw new ArgumentException(errorMessage ?? "Must be true.", argumentName);
        }
    }

    public static class GuardEnsure_GenericExtensions
    {
        public static void Null(this IGuardClause g, object input, string argumentName, string errorMessage = null)
        {
            if (input != null) throw new ArgumentException(errorMessage, argumentName);
        }

        public static void NotNull(this IGuardClause g, object input, string argumentName, string errorMessage = null)
        {
            if (input == null) throw new ArgumentNullException(errorMessage, argumentName);
        }

        public static void AreEqual(this IGuardClause g, object input, object other, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.Null(input, argumentName);
            if (!input.Equals(other)) throw new ArgumentException(errorMessage ?? $"Should equal '{other}'.", argumentName);
        }

        public static void AreNotEqual(this IGuardClause g, object input, object other, string argumentName, string errorMessage = null)
        {
            if (input != null && input.Equals(other)) throw new ArgumentException(errorMessage ?? $"Should NOT equal '{other}'.", argumentName);
        }

        public static void NotEmpty(this IGuardClause g, ValueType input, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            var empty = Activator.CreateInstance(input.GetType());
            Guard.Ensure.AreNotEqual(input, empty, argumentName, errorMessage);
        }

        public static void NotEmpty(this IGuardClause g, object input, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            var empty = Activator.CreateInstance(input.GetType());
            Guard.Ensure.AreNotEqual(input, empty, argumentName, errorMessage);
        }

        public static void NotEmpty(this IGuardClause g, IEnumerable input, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (!input.Cast<object>().Any()) throw new ArgumentException(errorMessage ?? "Must not be empty.", argumentName);
        }

        public static void NotEmpty<T>(this IGuardClause g, IEnumerable<T> input, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (!input.Any()) throw new ArgumentException(errorMessage ?? "Must not be empty.", argumentName);
        }
    }

    public static class GuardEnsure_StringExtensions
    {
        public static void NotEmpty(this IGuardClause g, string input, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (input.Trim().Length == 0) throw new ArgumentException(errorMessage ?? "Must not be empty.", argumentName);
        }

        public static void AreEqual(this IGuardClause g, string input, string other, StringComparison comparisonType, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotEmpty(input, argumentName, errorMessage);
            if (!input.Equals(other, comparisonType)) throw new ArgumentException(errorMessage ?? $"Must equal '{other}'.", argumentName);
        }

        public static void AreNotEqual(this IGuardClause g, string input, string other, StringComparison comparisonType, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotEmpty(input, argumentName, errorMessage);
            if (input.Equals(other, comparisonType)) throw new ArgumentException(errorMessage ?? $"Must NOT equal '{other}'.", argumentName);
        }

        public static void Length(this IGuardClause g, string input, int length, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (input.Length != length) throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must have a length of {length}.");
        }

        public static void MinLength(this IGuardClause g, string input, int minLength, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (input.Length < minLength) throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must have a minimum length of {minLength}.");
        }

        public static void MaxLength(this IGuardClause g, string input, int maxLength, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (input.Length > maxLength) throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must have a maximum length of {maxLength}.");
        }

        public static void Contains(this IGuardClause g, string input, string valueToSearch, StringComparison comparisonType, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (input.IndexOf(valueToSearch, comparisonType) < 0) throw new ArgumentException(argumentName, errorMessage ?? $"Must contain the value '{valueToSearch}'.");
        }

        public static void NotContains(this IGuardClause g, string input, string valueToSearch, StringComparison comparisonType, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotNull(input, argumentName, errorMessage);
            if (input.IndexOf(valueToSearch, comparisonType) >= 0) throw new ArgumentException(argumentName, errorMessage ?? $"Must contain the value '{valueToSearch}'.");
        }

        public static void IsContainedIn(this IGuardClause g, string input, string container, StringComparison comparisonType, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.Contains(container, input, comparisonType, argumentName, errorMessage);
        }

        public static void IsNotContainedIn(this IGuardClause g, string input, string container, StringComparison comparisonType, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotContains(container, input, comparisonType, argumentName, errorMessage);
        }
    }

    public static class GuardEnsure_ComparableExtensions
    {
        public static void GreaterThan<T>(this IGuardClause g, T input, T min, string argumentName, string errorMessage = null) where T : IComparable
        {
            if (input.CompareTo(min) <= 0)
                throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must be greater than {min}.");
        }

        public static void GreaterOrEqual<T>(this IGuardClause g, T input, T min, string argumentName, string errorMessage = null) where T : IComparable
        {
            if (input.CompareTo(min) < 0)
                throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must be greater than or equal to {min}.");
        }

        public static void LowerThan<T>(this IGuardClause g, T input, T max, string argumentName, string errorMessage = null) where T : IComparable
        {
            if (input.CompareTo(max) >= 0)
                throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must be lower than {max}.");
        }

        public static void LowerOrEqual<T>(this IGuardClause g, T input, T max, string argumentName, string errorMessage = null) where T : IComparable
        {
            if (input.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must be lower than or equal to {max}.");
        }

        public static void Between<T>(this IGuardClause g, T value, T min, T max, string argumentName, string errorMessage = null) where T : IComparable
        {
            if (!(value.CompareTo(min) > 0 && value.CompareTo(max) < 0))
                throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must be a value between {min} and {max} (not inclusive).");
        }

        public static void BetweenInclusive<T>(this IGuardClause g, T value, T min, T max, string argumentName, string errorMessage = null) where T : IComparable
        {
            if (!(value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0))
                throw new ArgumentOutOfRangeException(argumentName, errorMessage ?? $"Must be a value between {min} and {max} (inclusive).");
        }
    }

    public static class GuardEnsure_Expressions
    {
        public static void Rule(this IGuardClause g, Func<bool> func, string argumentName, string errorMessage = null)
        {
            if (!func()) throw new ArgumentException(errorMessage ?? "Value is not valid.", argumentName);
        }
    }

    public static class GuardEnsure_EmailExtensions
    {
        public static void IsValidEmail(this IGuardClause g, string input, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotEmpty(input, argumentName, errorMessage);
            try { new MailAddress(input); }
            catch { throw new ArgumentException(errorMessage ?? "Is not a valid email.", argumentName); }
        }
    }

    public static class GuardEnsure_UriExtensions
    {
        public static void IsValidUrl(this IGuardClause g, string input, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.NotEmpty(input, argumentName, errorMessage);
            if (!Uri.IsWellFormedUriString(input, UriKind.Absolute)) throw new ArgumentException(errorMessage ?? "Is not a valid URL.", argumentName);
        }
    }

    public static class GuardEnsure_RegexExtensions
    {
        public static void IsMatch(this IGuardClause g, string input, Regex regex, string argumentName, string errorMessage = null)
        {
            if (!regex.IsMatch(input))
                throw new ArgumentException(errorMessage ?? $"'{input}' is not a valid value.", argumentName);
        }

        public static void IsMatch(this IGuardClause g, string input, string regexPattern, string argumentName, string errorMessage = null)
        {
            Guard.Ensure.IsMatch(input, new Regex(regexPattern), argumentName, errorMessage);
        }
    }
}
