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

Concrete rule types are serialized inside the database, so any refactoring that will change their name or namespace introduce a breaking change (and requires a db migration).

## Setup

This project requires .NET 5. 

### Build

To build the project move in the root directory and run:

```
dotnet build
```


### Tests

To run units and integration tests move in the root directory and run:

```
dotnet test
```

### Database

The projects uses a SQL Lite db on a local file. To initialize the database move inside the root folder and run:

```
dotnet ef database update --project src/EntityFrameworkCore --startup-project src/Web.Host --verbose
```

### Run

Once built you can run the project by running the command (in the `src/Web.Host`):

```
dotnet run
```

You should be able to see swagger at [https://localhost:5001/swagger](https://localhost:5001/swagger).
