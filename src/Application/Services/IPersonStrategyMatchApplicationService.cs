using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;

namespace Application.Services
{
    public interface IPersonStrategyMatchApplicationService
    {
        Task CreateStrategy(CreateStrategyDto input);

        Task<StrategyDto> GetByIdAsync(Guid strategyId);

        Task DeleteStrategy(Guid id);
    }
}
