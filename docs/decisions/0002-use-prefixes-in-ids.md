# Use prefixes in IDs

## Status

* Status: **accepted**
* Deciders: [alex1ozr](https://github.com/alex1ozr)
* Date: 16.03.2024
* Technical Story: SPACE-14

## Context and Problem Statement

The common practice is to use numeric or GUID identifiers.
While we have [decided](0001-use-strings-as-ids.md) to use strings as IDs, they can be just serialized UUIDs or other random strings.
It is not user- or DB-friendly in many cases:
- It does not provide any information about the entity type
- It does not help with DB scalability

## Considered Options

* Use plain UUIDs
* Use composite keys with prefixes
  * Default ID template: `{service}-{module}-{entity}-{random UUID encoded as base64}`
    * Example: `sw-pdi-ace-_hTUleJcPkSbEAiVJLnxNA`

## Decision Outcome

Chosen option: "Use composite keys with prefixes", because it is more flexible.
- Strings can encode different information, such as the type of the entity, which can be useful for debugging and auditing purposes.
- Composite keys can be effectively represented using strings.
- It is possible to shard the database by the prefix or other string part.

## Links

* [5 Tips for API Design](https://codeopinion.com/want-to-build-a-good-api-here-are-5-tips-for-api-design/)
* [How to (and how not to) design REST APIs](https://github.com/stickfigure/blog/wiki/How-to-(and-how-not-to)-design-REST-APIs?ref=vladimir-ivanov-dev-blog#rule-6-do-use-strings-for-all-identifiers)
