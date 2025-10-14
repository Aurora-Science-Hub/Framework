# Use strings as IDs

## Status

* Status: **accepted**
* Deciders: [alex1ozr](https://github.com/alex1ozr)
* Date: 16.03.2024
* Technical Story: SPACE-14

## Context and Problem Statement

The common practice is to use numeric or GUID identifiers.
However, both approaches have their own set of challenges.
- Numeric identifiers, especially if they are auto-incrementing, can pose a scalability issue.
If the number of records exceeds the maximum value that the numeric type can hold, it can lead to problems.
  - They are often sequential and can be easily guessed, leading to potential security vulnerabilities if the IDs are exposed to the user.
- On the other hand, UUIDs, are designed to be globally unique.
  - They are also larger than numeric identifiers, which can have implications for database performance and storage.
  - UUIDs are not human-friendly, hard to remember, and prone to errors when manually inputting.
  - They also lack a natural sort order.

## Considered Options

* Int
* UUID
* String

## Decision Outcome

Chosen option: "String", because it is more flexible and allows for the use of prefixes.
- Strings can encode different information, such as the type of the entity, which can be useful for debugging and auditing purposes.
- Composite keys can be effectively represented using strings.
- The use of numeric IDs can limit the flexibility and options for future developers.

## Links

* [5 Tips for API Design](https://codeopinion.com/want-to-build-a-good-api-here-are-5-tips-for-api-design/)
* [How to (and how not to) design REST APIs](https://github.com/stickfigure/blog/wiki/How-to-(and-how-not-to)-design-REST-APIs?ref=vladimir-ivanov-dev-blog#rule-6-do-use-strings-for-all-identifiers)
