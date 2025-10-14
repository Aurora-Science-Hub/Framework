# Implement solution as modular monolith

## Status

* Status: **accepted**
* Deciders: [alex1ozr](https://github.com/alex1ozr)
* Date: 16.03.2024
* Technical Story: SPACE-14

## Context and Problem Statement

We are developing a new software system that needs to be scalable, maintainable, and easy to understand.
The architecture of the system is a critical decision that will impact these aspects.
There are several architectural styles we could adopt, each with its own trade-offs.
- A monolithic architecture, where the entire application is built as a single unit, can be simple to develop and test but may become complex and difficult to understand as the system grows.
  - It can also be challenging to scale and deploy.
- A microservices architecture, where the application is split into small, independently deployable services,
can be easier to understand and scale but introduces complexity in terms of service coordination and data consistency.
  - It can also be challenging to test and deploy.
  - It requires a team with a high level of expertise in distributed systems.
- A modular monolith architecture attempts to strike a balance between these extremes.
  - It structures the application as a set of loosely coupled modules within a single deployable unit.
  - This approach aims to provide the simplicity and consistency of a monolith with the modularity and scalability of microservices.

## Considered Options

* Monolith
* Modular monolith
* Microservices

## Decision Outcome

Chosen option: "Modular monolith", because it provides a good balance between simplicity and scalability.

## Links

- [MonolithFirst by Martin Fowler](https://martinfowler.com/bliki/MonolithFirst.html)
- [Modular Monoliths - Simon Brown](https://files.gotocon.com/uploads/slides/conference_12/515/original/gotoberlin2018-modular-monoliths.pdf)
- [Modular Monolith Architecture](https://awesome-architecture.com/modular-monolith/)
