using System;

namespace Application.Extensions
{
    /// <summary>
    /// Extensions for simplifying guid handling.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Converts a string guid to a <see cref="Guid"/> or throws an exception if not possibile.
        /// </summary>
        public static Guid ToGuid(this string guid)
        {
            return Guid.Parse(guid);
        }
    }
}
