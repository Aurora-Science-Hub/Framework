using AuroraScienceHub.Framework.AspNetCore.Problems.Enrichers;
using AuroraScienceHub.Framework.Composition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.AspNetCore.Problems;

/// <summary>
/// <see cref="IServiceCollection"/> extensions for exception to problem details conversion
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the mechanism that converts all exceptions to detailed problems
    /// </summary>
    public static IServiceCollection AddExceptionAsProblem(this IServiceCollection services)
    {
        return services
            .AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = FormatProblem;
            })
            .AddTransientAsImplementedInterfaces<DefaultExceptionProblemEnricher>()
            .AddTransientAsImplementedInterfaces<ValidationProblemEnricher>()
            .AddTransient<ProblemDetailsFormatter>()
            .AddExceptionHandler<ExceptionLoggingHandler>();
    }

    /// <summary>
    /// Use the mechanism that converts all exceptions to detailed problems
    /// </summary>
    public static IApplicationBuilder UseExceptionAsProblem(this IApplicationBuilder app, bool useExceptionPage)
    {
        app.UseExceptionHandler();

        if (useExceptionPage)
        {
            app.UseDeveloperExceptionPage();
        }

        return app;
    }

    private static void FormatProblem(ProblemDetailsContext context)
    {
        var httpContext = context.HttpContext;
        var handlerFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        var exception = handlerFeature?.Error;
        var problem = context.ProblemDetails;
        var formatter = httpContext.RequestServices.GetRequiredService<ProblemDetailsFormatter>();

        formatter.Format(problem, exception, httpContext);
    }
}
