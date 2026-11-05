using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});
builder.Services.AddAi(builder.Configuration);

using var host = builder.Build();
var assistant = host.Services.GetRequiredService<IAiAssistant>();

var question = args.Length > 0
    ? String.Join(' ', args)
    : "Explain in three sentences why the aurora appears near the polar regions.";

var request = new AiRequest(
    Instructions: "You are a space weather guide. Give accurate and concise answers.",
    Summary: null,
    History: [],
    Question: question)
{
    Options = new ChatOptions { MaxOutputTokens = 300 },
};

var result = await assistant.AskAsync(request, CancellationToken.None);
if (!result.IsSuccess)
{
    Console.Error.WriteLine($"The assistant did not answer: {result.Outcome}");
    return 1;
}

Console.WriteLine(result.Text);
if (result.Usage is not null)
{
    Console.WriteLine();
    Console.WriteLine($"Tokens: {result.Usage.InputTokens} input, {result.Usage.OutputTokens} output.");
}

return 0;
