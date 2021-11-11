using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;

namespace Domain.Extensions
{
    /// <summary>
    /// Extensions to help validating parameters.
    /// </summary>
    public static class DomainExtensions
    {
        /// <summary>
        /// Returns an empty Enumerable if the given input is null, otherwise it returns the original input.
        /// </summary>
        /// <param name="collection">The collection to check for null.</param>
        /// <typeparam name="T">The collection's generic parameter.</typeparam>
        /// <returns>An empty Enumerable if the given input is null.</returns>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> collection)
        {
            if (collection is null)
            {
                return Enumerable.Empty<T>();
            }

            return collection;
        }

        /// <summary>
        /// Throws a <see cref="ValidationException"/> if the object is null.
        /// </summary>
        /// <param name="obj">The object to check for null.</param>
        /// <param name="argumentName">The argument name to pass to the <see cref="ValidationException"/></param>
        /// <returns>The original object.</returns>
        /// <exception cref="ValidationException">When the input is null.</exception>
        public static T ThrowIfNull<T>(this T obj, string argumentName)
        {
            if (obj is null)
            {
                throw new ValidationException($"{argumentName} cannot be null.");
            }

            return obj;
        }

        /// <summary>
        /// Throws a <see cref="ValidationException"/> if the string is null, empty or whitespace.
        /// </summary>
        /// <param name="str">The string to check for null, empty or whitespace.</param>
        /// <param name="argumentName">The argument name to pass to the <see cref="ValidationException"/></param>
        /// <returns>The original string.</returns>
        /// <exception cref="ValidationException">When the input string is null, empty or whitespace.</exception>
        public static string ThrowIfNullOrEmpty(this string str, string argumentName)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                throw new ValidationException($"{argumentName} cannot be null or empty.");
            }

            return str;
        }

        /// <summary>
        /// Throws a <see cref="ValidationException"/> if the collection is null or empty.
        /// </summary>
        /// <param name="collection">The collection to check for null or empty.</param>
        /// <param name="argumentName">The argument name to pass to the <see cref="ValidationException"/></param>
        /// <returns>The original collection.</returns>
        /// <exception cref="ValidationException">When the collection is null or empty.</exception>
        public static IList<T> ThrowIfNullOrEmpty<T>(this IList<T> collection, string argumentName)
        {
            if (collection is null || !collection.Any())
            {
                throw new ValidationException($"{argumentName} cannot be null or empty.");
            }

            return collection;
        }
    }
}
