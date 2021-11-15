using System;
using System.Text;
using Domain.Rules;

namespace Application.Cache
{
    internal static class CacheKeys
    {
        /// <summary>
        /// Returns the cache key sorting the guids and combining them as "CalculateProbabilitySameIdentity.Guid1.Guid2.StrategyId".
        /// </summary>
        public static string CalculateProbabilitySameIdentity(
            Guid firstPersonId,
            Guid secondPersonId,
            Guid strategyId)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(nameof(CalculateProbabilitySameIdentity));
            stringBuilder.Append('.');

            var firstGuid = firstPersonId.ToString();
            var secondGuid = secondPersonId.ToString();

            if (string.CompareOrdinal(firstGuid, secondGuid) <= 0)
            {
                stringBuilder.Append(firstGuid);
                stringBuilder.Append('.');
                stringBuilder.Append(secondGuid);
            }
            else
            {
                stringBuilder.Append(secondGuid);
                stringBuilder.Append('.');
                stringBuilder.Append(firstGuid);
            }

            stringBuilder.Append('.');
            stringBuilder.Append(strategyId.ToString());

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Cache key for available list of <see cref="IMatchingRuleContributor"/>.
        /// </summary>
        public const string AvailableRules = nameof(AvailableRules);
    }
}
