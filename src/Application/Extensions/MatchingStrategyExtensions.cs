using System.Linq;
using Application.Contracts;
using Domain.Extensions;
using Domain.Model;

namespace Application.Extensions
{
    /// <summary>
    /// Same extension to convert a domain model to a dto. This should be done using automapper.
    /// </summary>
    public static class MatchingStrategyExtensions
    {
       /// <summary>
       /// Converts the domain model to a dto.
       /// </summary>
       public static StrategyDto ToDto(this MatchingStrategy strategy)
        {
            return new StrategyDto()
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Description = strategy.Description,
                Rules = strategy.Rules.Select(r =>
                {
                    return new StrategyDto.RuleDto()
                    {
                        Name = r.Name,
                        Description = r.Description,
                        IsEnabled = r.IsEnabled,
                        RuleTypeAssemblyQualifiedName = r.RuleType.GetAssemblyQualifiedName(),
                        Parameters = r.Parameters.Select(p =>
                        {
                            return new StrategyDto.RuleDto.ParameterDto()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Value = p.Value,
                            };
                        }),
                    };
                }),
            };
        }
    }
}
