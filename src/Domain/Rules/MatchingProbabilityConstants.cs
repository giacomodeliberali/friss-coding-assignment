using Domain.Model;

namespace Domain.Rules
{
    /// <summary>
    /// The matching probability constants.
    /// </summary>
    public static class MatchingProbabilityConstants
    {
        /// <summary>
        /// Represents the value that indicates that there is a match with probability 1 (100%) between the two <see cref="Person"/>.
        /// </summary>
        public const decimal Match = 1;

        /// <summary>
        /// Represents the value that indicates that there is no match (0%) between the two <see cref="Person"/>.
        /// </summary>
        public const decimal NoMatch = 0;
    }
}
