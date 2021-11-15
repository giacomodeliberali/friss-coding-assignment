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

public interface IMatchingRuleContributor
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

### MatchingRule configuration

MatchingRules contains domain logic, thus they are considered domain services. This means that a client can not create
its own rule on demand, but it can only use existing rules and can possibly tune rule's parameters to alter its behaviour.

What a client can do is:
- Create/update a strategy choosing the list of rules that it wants and their execution order
- Create/update a strategy disabling one or more of its rules
- Create/update a strategy providing a list of custom parameters for each rule
  - Every rule has its own parameters that a client can manipulate. If unspecified each rule will use default values

Example: create a strategy specifying a rule parameter 
```json
{
    "name": "My custom strategy",
    "description": "A strategy that has only a single rule on the last name that return 80% on match",
    "rules": [
        {
            "name": "LastNameMatchingMatchingRule",
            "description": "This rule add 80% if the last names match",
            "isEnabled": true,
            "ruleTypeAssemblyQualifiedName": "Application.Rules.LastNameMatchingMatchingRule, Application",
            "parameters": [
                {
                    "name": "IncreaseProbabilityWhenEqualsLastNames",
                    "value": 0.8
                }
            ]
        }
    ]
}
```

The list of available rules and their parameters can be seen by client by invoking the endpoint `GET api/strategies/available-rules`. An example response will be
```json
[
    {
        "assemblyQualifiedName": "Application.Rules.FirstNameMatchingMatchingRule, Application",
        "description": "This rule add 20% if the first names match or 15% if they are similar.",
        "parameters": [
            {
                "name": "IncreaseProbabilityWhenEqualsFirstNames",
                "description": "The probability to add for a first name exact match."
            },
            {
                "name": "IncreaseProbabilityWhenSimilarFirstNames",
                "description": "The probability to add for a first name similarity match."
            }
        ]
    }
]
```

Every rule have to specify the list of parameters it allows as configuration by using the `RuleParameter` attribute.
The description of the parameter will be sent to clients for documentation purposes as well as the class XML docs comments.

Example: 

```csharp
/// <summary>
/// This rule add 20% if the first names match or 15% if they are similar.
/// </summary>
[RuleParameter(IncreaseProbabilityWhenEqualsFirstNames, "The probability to add for a first name exact match.")]
[RuleParameter(IncreaseProbabilityWhenSimilarFirstNames, "The probability to add for a first name similarity match.")]
public class FirstNameMatchingMatchingRule : IMatchingRuleContributor
{
  // ...
 }
```

### Logs

Application logs are written to console and to file (`src/Web.Host/Logs/logs.txt`) using Serilog. All
handled exception will be logged as *Warning*, while unhandled exceptions will be logged as *Error*.

### Caching

Requests caching is implemented with an in-memory structure (`IMemoryCache`), and thus it gets cleared at every app restart.
It also doesn't support scaling as it is not distributed (eg. *Redis*). Requests that are cached at the moment are:
- `api/strategies/available-rules`: the list of available *MatchingRule*s never change after app start, so it is cached indefinitely
- `api/people/probability-same-identity`: the probability never changes until either a person or the strategy are update/deleted. The request is cached (sorting the ids) and removed once the strategy changes (people don't have a full CRUD yet). 

The default interface `IMemoryCache` is wrapped in a custom `ICustomMemoryCache` that just wraps the default implementation `MemoryCache` and exposes the list of keys to allow a batch deletion.

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

This will create a local file named `FrissCodingAssignment.db` and it will seed the given rules, strategy and people.

### Run

Once the database has been created you can run the project by typing the command (in the root folder):

```
dotnet run --project src/Web.Host
```

You should be able to see swagger at [https://localhost:5001/swagger](https://localhost:5001/swagger).

# Postman

To quickly run the project to see how it works I included a Postman collection with the needed requests to simulate the tests written in the assignment. 
You can find the [collection](./Default.postman_collection.json) inside the root folder (`Default.postman_collection.json`). You can [import it directly into postman](https://learning.postman.com/docs/getting-started/importing-and-exporting-data/#importing-data-into-postman) to try it out.

The workflow to test the rules in the assignment is:
- Call the `Get All Strategies` that should return the single previously seeded strategy with rules (this request will populate the collection variable `DefaultStrategyId` used in subsequent requests)
- Call the different endpoints under the folder `CalculateProbability` that represent the examples provided in the assignment (it uses the previously seeded people as well)
