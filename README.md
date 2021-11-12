# FRISS Coding Assignment

## Summary

We want you to create 2 endpoints:
- Receives and stores a person
- Calculates the probability that 2 persons are the same physical person.

A person is represented by the following attributes:
- First name
- Last name
- Date of birth (can be unknown)
- Identification number (can be unknown)

The matching logic should work in the following way:
- If the Identification number matches then 100%
- Otherwise:
  - If the lastname is the same +40% 
  - If the first name is the same +20% 
    - If the first name is similar +15% (see examples)
  - If the date of birth matches +40%
  - If the dates of birth are known and not the same, there is no match

Similar first names examples:
- Andrew and A. (initials)
- Andrew and Andew (typo)
- Andrew and Andy (diminutive)

Here are some other points: Logging, Documentation, Security, Request caching, Mathing rules are configurable, UI
for matching rule configuration. They are in order of importance to us!

## Implementation

I have extracted a few domain models:
- *Person*: the person that we are comparing for fraudulent identity.
- *MatchingRule*: contains the logic to determine if two given people have the same identity. A MatchingRule can have a list of parameters that can be used to customize the runtime behaviour of the rule (eg. change the default probabilities).
- *MatchingStrategy*: a collection of MatchingRules that specifies the order of executions and that is responsible of executing the rules.

As rules' logic might have dependencies or rely on external services, their logic doesn't fit well in the domain project.
Each *MatchingRule* wraps indeed a concrete *Application* type that implements the *IMatchingRuleContributor*:

```csharp
namespace Domain.Rules;

public interface IRuleContributor
{
    Task<ProbabilitySameIdentity> MatchAsync(
      MatchingRule rule,
      Person first,
      Person second,
      ProbabilitySameIdentity currentProbability,
      NextMatchingRuleDelegate next);
}
```

A *MatchingStrategy* is represented by a collection of *MatchingRules*.
The rules inside a strategy are executed in a pipeline fashion where,
starting by the first rule, each of them is responsible for 
either terminating the pipeline returning a result, 
or continue invoking the next rule (very similar concept to .NET Core middleware).

The component that actually executes a strategy invoking its rules implements the interface `IMatchingRuleStrategyExecutor`.

```
namespace Domain.Rules;

public interface IMatchingRuleStrategyExecutor
{
    Task<ProbabilitySameIdentity> ExecuteAsync(
      MatchingStrategy strategy,
      Person first,
      Person second);
}
```

### MatchingRule discovery

All *MatchingRule*s are discovered at runtime exploiting assembly scanning.
To add a new rule simply create a new class that implements the `IRuleContributor`, it will be automatically
registered into the DI container and made visible to clients.
