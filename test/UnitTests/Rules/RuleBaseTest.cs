using System.Threading.Tasks;
using Domain.Rules;
using NSubstitute;

namespace UnitTests.Rules
{
    public class RuleBaseTest
    {
        /// <summary>
        /// A mock for the next delegate that returns its input.
        /// </summary>
        protected readonly NextMatchingRuleDelegate NextMatchingRuleDelegate;

        public RuleBaseTest()
        {
            NextMatchingRuleDelegate = Substitute.For<NextMatchingRuleDelegate>();

            // mock to return input
            NextMatchingRuleDelegate.Invoke(Arg.Any<decimal>()).ReturnsForAnyArgs(x => Task.FromResult(x.Arg<decimal>()));
        }
    }
}
