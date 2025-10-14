# Use Images as Base64 string 

## Status

* Status: **accepted**
* Deciders: [pavelkudriavtsevsw](https://github.com/pavelkudriavtsevsw)
* Date: 10.05.2024
* Technical Story: SPACE-43

## Context and Problem Statement

The common practice is to use byte[] or base64 string for images.
Both approaches have their advantages and disadvantages.
Directly using binary data for certain tasks in web development can present several issues. Here are some reasons why binary data might not be directly used, and instead, base64 or other encoding mechanisms are preferred:
- Compatibility: Not all systems are equipped to handle raw binary data appropriately. Certain binary characters can be perceived as control characters (like newline, carriage return, or end of file), which might disrupt the data processing. Encoding binary data into a format like base64 ensures that it can be safely transported and processed regardless of the system in use.
- Transmission Issues: Certain protocols might not handle binary data effectively during data transmission. For instance, email protocols were originally designed for text data, not binary. Binary data could be unintentionally modified or corrupted during transmission, making it unusable at its destination.
- Embedding Limitations: Web development often requires embedding data (such as images or fonts) directly into HTML, CSS, or JavaScript files. These files are text-based, and binary data can’t be directly inserted into them since it’s akin to writing in a language the browser cannot understand. Base64 encoding allows this binary data to be represented as a text string, which can be easily embedded.
- URL Encoding: URLs are designed to handle a limited set of characters. Binary data can’t be included in URLs because it can contain any byte value, many of which are not safe or valid in a URL. Encoding binary data into base64 allows it to be safely included in a URL.

## Considered Options

* Base64 String
* Image as byte[]

## Decision Outcome

Chosen option: "Base64 String", because it is more flexible and compatible for purposes of the project.

## Links

* [A Dive into Base64 and Its Significance in Web Development](https://medium.com/@shimonmoyal/a-dive-into-base64-and-its-significance-in-web-development-b6bd4427f61d)
