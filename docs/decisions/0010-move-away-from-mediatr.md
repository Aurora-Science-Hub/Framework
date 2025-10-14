# Move away from MediatR

## Status

* Status: accepted

## Replace MediatR with Mediator library

* Deciders: Alex
* Date: 2025-09-12

Technical Story: Migration from MediatR to Mediator library for the SWeather application

## Context and Problem Statement

MediatR has become a commercial product, requiring licensing fees for continued use in production environments.
Additionally, the application would benefit from better performance characteristics in the mediator pattern implementation.
How can we maintain the mediator pattern benefits while avoiding licensing costs and improving performance?

## Decision Drivers

* MediatR licensing change to commercial model increases operational costs
* Need to maintain existing mediator pattern architecture
* Performance optimization opportunities through source generators
* Desire to use open-source alternatives where possible
* Minimal code changes required for migration

## Considered Options

* Continue using MediatR with commercial license
* Replace with Mediator library (using source generators)
* Implement custom mediator pattern
* Remove mediator pattern entirely and use direct dependencies

## Decision Outcome

Chosen option: "Replace with Mediator library", because it provides the same functionality as MediatR while being open-source and offering better performance through source generators. The migration effort is minimal due to similar APIs.

### Positive Consequences

* Eliminates licensing costs associated with commercial MediatR
* Improved performance through compile-time source generation instead of runtime reflection
* Maintains existing architecture patterns and code structure
* Continues to provide clean separation of concerns and CQRS benefits
* Open-source solution with active community support

### Negative Consequences

* Migration effort required to replace MediatR references
* Potential minor API differences requiring code adjustments
* Team needs to learn new library specifics (though minimal due to similar interface)

## Pros and Cons of the Options

### Continue using MediatR with commercial license

* Good, because no migration effort required
* Good, because team is already familiar with the library
* Bad, because introduces ongoing licensing costs
* Bad, because performance characteristics remain suboptimal due to reflection usage

### Replace with Mediator library

* Good, because eliminates licensing costs
* Good, because provides better performance through source generators
* Good, because maintains familiar mediator pattern
* Good, because minimal migration effort due to similar API
* Good, because open-source with active development
* Bad, because requires migration effort
* Bad, because slight learning curve for new library specifics

### Implement custom mediator pattern

* Good, because full control over implementation
* Good, because no external dependencies
* Bad, because significant development effort required
* Bad, because need to maintain custom implementation
* Bad, because potential for bugs in custom implementation

### Remove mediator pattern entirely

* Good, because eliminates external dependency
* Bad, because requires major architectural changes
* Bad, because loses clean separation of concerns
* Bad, because significant refactoring effort required

## Links

* [Mediator library GitHub](https://github.com/martinothamar/Mediator)
* [MediatR licensing announcement](https://github.com/jbogard/MediatR)
