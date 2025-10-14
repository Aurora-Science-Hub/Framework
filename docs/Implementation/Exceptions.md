# Exceptions

We agreed that all errors in the application will be implemented as exceptions. Even user input validation errors.

Therefore, all problems or errors within the application are **Exceptions**.

## Requirements
All exceptions should be represented as [ProblemDetails](https://datatracker.ietf.org/doc/html/rfc9457) outside the application.
- ProblemDetails is public information.
- It should not contain sensitive data.
- Exceptions from previous rule can be made for test stands (for example, the stack of the program).

## Classification
All exceptions can be divided into:
- Expected user input errors (Validation Exception) or unexpected application errors (Unexpected Exception)
- Temporary (which may disappear over time) or permanent (which, with the same input data, lead to the same problem)

### Expected and unexpected exceptions
How to determine which type of error to throw:
- If the exception is the result of a regular user input check, then it is a **Validation Exception**.
- If as a result of an exception being triggered, it is necessary to create an issue for the developer and go
fix something in the application (data, integration, settings, environment configuration, etc.), then it is an **Unexpected Exception**.

#### Validation Exception
- Exceptions that the user should see (Validation Exception) should be as detailed as possible.
- They should unambiguously indicate the problem that led to the error.
- Messages in Validation exceptions are written for users. Therefore, there should be no technical information, such as
  identifiers.
  -  On the other hand, if we failed to parse the value, we should tell the user this value. So that he can change
     it somehow. That is, in the case of the `Parse` method failure, this is not an identifier in the message, it is an invalid
     value, and it can be shown.

#### Not found
**NotFoundException** is a special case of ValidationException.
When the required entity is not found, it is necessary to throw the `NotFoundException`. It can be thrown from the lowest
level — from the Data layer.

All Repositories that have a load of an entity by identifier must throw `NotFoundException` if the entity is not found.
If the entity is not required to be loaded (by business logic), this exception should not be thrown.
Instead of throwing this exception, it is necessary to add a new method to the repository (`GetEntityOrDefault`)

#### Unexpected Exception
- It is recommended to insert more technical details that can help to facilitate correction.
- In user, on the contrary, avoid unnecessary complications and disclosure of implementation.

## Mapping to HTTP level
Exception types should be divided only when someone needs it. The main consumer of these differences will be the web
layer and its http status codes. Different exceptions will lead to different return codes.

| Exception           | HTTP name           | HTTP status code |
|---------------------|---------------------|------------------|
| NotFoundException   | NotFound            | 404              |
| ValidationException | BadRequest          | 400              |
| UnexpectedException | InternalServerError | 500              |


## See also
- [Error categories and category errors by Mark Seemann](https://blog.ploeh.dk/2024/01/29/error-categories-and-category-errors/)
- [CQRS and exception handling](https://enterprisecraftsmanship.com/posts/cqrs-exception-handling/)
