namespace AuroraScienceHub.Framework.Ai.Deepl;

/// <summary>
/// DeepL client
/// </summary>
public interface IDeeplClient
{
    /// <summary>
    /// Translates the given text from the source language to the target language.
    /// </summary>
    Task<string?> TranslateAsync(
        string text,
        DeeplLanguage sourceLanguage,
        DeeplLanguage targetLanguage,
        CancellationToken cancellationToken);
}
