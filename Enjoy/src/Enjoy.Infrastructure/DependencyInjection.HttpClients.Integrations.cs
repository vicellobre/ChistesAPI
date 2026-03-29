using System.Net.Http.Headers;
using Enjoy.Application.Abstractions.ExternalAuth;
using Enjoy.Application.Abstractions.Jokes;
using Enjoy.Infrastructure.Auth;
using Enjoy.Infrastructure.HttpClients.Options;
using Enjoy.Infrastructure.Jokes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Enjoy.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructureIntegrationHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IExternalAuthProvider, ExternalAuthProvider>((sp, client) =>
        {
            IntegrationHttpClientsOptions root = sp.GetRequiredService<IOptionsMonitor<IntegrationHttpClientsOptions>>()
                .CurrentValue;
            ExternalAuthHttpClientOptions opt = root.ExternalAuth;
            if (!string.IsNullOrWhiteSpace(opt.AcceptHeaderName) && !string.IsNullOrWhiteSpace(opt.AcceptHeaderValue))
            {
                client.DefaultRequestHeaders.Add(opt.AcceptHeaderName, opt.AcceptHeaderValue);
            }

            client.Timeout = HttpClientTimeoutFromOptions(
                opt.RequestTimeoutSeconds,
                opt.RequestTimeoutMinSeconds,
                opt.RequestTimeoutMaxSeconds);
        });

        services.AddHttpClient<IChuckNorrisJokeService, ChuckNorrisJokeService>((sp, client) =>
        {
            IntegrationHttpClientsOptions root = sp.GetRequiredService<IOptionsMonitor<IntegrationHttpClientsOptions>>()
                .CurrentValue;
            ChuckNorrisHttpClientOptions opt = root.ChuckNorris;
            string baseUrl = string.IsNullOrWhiteSpace(opt.BaseUrl)
                ? ChuckNorrisHttpClientOptions.DefaultBaseUrl
                : opt.BaseUrl;
            client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            client.Timeout = HttpClientTimeoutFromOptions(
                opt.RequestTimeoutSeconds,
                opt.RequestTimeoutMinSeconds,
                opt.RequestTimeoutMaxSeconds);
        });

        services.AddHttpClient<IDadJokeService, DadJokeService>((sp, client) =>
        {
            IntegrationHttpClientsOptions root = sp.GetRequiredService<IOptionsMonitor<IntegrationHttpClientsOptions>>()
                .CurrentValue;
            DadJokeHttpClientOptions opt = root.DadJoke;
            string baseUrl = string.IsNullOrWhiteSpace(opt.BaseUrl)
                ? DadJokeHttpClientOptions.DefaultBaseUrl
                : opt.BaseUrl;
            client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            client.DefaultRequestHeaders.Accept.Clear();
            if (!string.IsNullOrWhiteSpace(opt.AcceptMediaType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(opt.AcceptMediaType));
            }

            client.DefaultRequestHeaders.Remove("User-Agent");
            string userAgent = opt.UserAgent.Trim();
            if (!string.IsNullOrEmpty(userAgent))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
            }

            client.Timeout = HttpClientTimeoutFromOptions(
                opt.RequestTimeoutSeconds,
                opt.RequestTimeoutMinSeconds,
                opt.RequestTimeoutMaxSeconds);
        });

        return services;
    }

    private static TimeSpan HttpClientTimeoutFromOptions(int requestSeconds, int minSeconds, int maxSeconds)
    {
        int min = Math.Min(minSeconds, maxSeconds);
        int max = Math.Max(minSeconds, maxSeconds);
        int seconds = Math.Clamp(requestSeconds, min, max);
        return TimeSpan.FromSeconds(seconds);
    }
}
