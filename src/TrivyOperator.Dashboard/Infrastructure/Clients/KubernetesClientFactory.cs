using k8s;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Infrastructure.Clients;

public class KubernetesClientFactory : IKubernetesClientFactory
{
    private readonly Kubernetes kubernetesClient;
    private ILogger logger;

    public KubernetesClientFactory(IOptions<KubernetesOptions> options, ILogger<KubernetesClientFactory> logger)
    {
        this.logger = logger;

        string? kubeconfigFileName = options.Value.KubeConfigFileName;
        if (!string.IsNullOrWhiteSpace(kubeconfigFileName))
        {
            try
            {
                if (File.Exists(kubeconfigFileName))
                {
                    KubernetesClientConfiguration config = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeconfigPath: kubeconfigFileName);
                    kubernetesClient = new Kubernetes(config, new PolicyHttpMessageHandler(GetRetryPolicy()));
                }
                else
                {
                    logger.LogWarning("Provided kubeconfig file does not exist: {kubeconfigFileName}", kubeconfigFileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Could not use the provided kubeconfig file: {kubeconfigFileName}", kubeconfigFileName);
                logger.LogWarning(ex, "Source: {exceptionSource}. Error message: {exceptionMessage}", ex.Source, ex.Message);
                logger.LogWarning("Falling back to standard creation of Kubernetes Client.");
            }
        }

        if (kubernetesClient is null) // kubeconfigFileName is not IsNullOrWhiteSpace OR something bad happened
        {
            KubernetesClientConfiguration? defaultConfig = KubernetesClientConfiguration.IsInCluster()
                    ? KubernetesClientConfiguration.InClusterConfig()
                    : KubernetesClientConfiguration.BuildConfigFromConfigFile();
            kubernetesClient = new Kubernetes(defaultConfig, new PolicyHttpMessageHandler(GetRetryPolicy()));
        }
    }

    public Kubernetes GetClient() => kubernetesClient;

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
