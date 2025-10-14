using DeepL;

namespace Ai.Deepl;

internal sealed class DeeplClient : IDeeplClient
{
    private readonly Translator _translator;

    public DeeplClient(Translator translator)
    {
        _translator = translator;
    }

    public async Task<string?> TranslateAsync(
        string text,
        DeeplLanguage sourceLanguage,
        DeeplLanguage targetLanguage,
        CancellationToken cancellationToken)
    {
        var result = await _translator.TranslateTextAsync(
                text,
                GetLanguageString(sourceLanguage),
                GetLanguageString(targetLanguage),
                options: new TextTranslateOptions { PreserveFormatting = true, },
                cancellationToken)
            .ConfigureAwait(false);

        return result?.Text;
    }

    private static string GetLanguageString(DeeplLanguage language)
    {
        return language switch
        {
            DeeplLanguage.English => "EN",
            DeeplLanguage.Russian => "RU",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
}
