using k8s;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Net;
using System.Xml.Linq;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Infrastructure.Clients;

public class K8sClientFactory: IK8sClientFactory
{
    private readonly Kubernetes k8sClient;
    private IConfiguration configuration;
    private ILogger logger;

    public K8sClientFactory(IConfiguration configuration, ILogger<K8sClientFactory> logger)
    {
        this.configuration = configuration;
        this.logger = logger;

        // TODO: change from IConfiguration to IOptions
        string kubeconfigFileName = configuration.GetSection("Kubernetes").GetValue<string>("KubeConfigFileName");

        if (!string.IsNullOrWhiteSpace(kubeconfigFileName))
        {
            try
            {
                if (File.Exists(kubeconfigFileName))
                {
                    KubernetesClientConfiguration config = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeconfigPath: kubeconfigFileName);
                    k8sClient = new Kubernetes(config, new PolicyHttpMessageHandler(GetRetryPolicy()));
                }
                else
                {
                    logger.LogWarning($"Provided kubeconfig file does not exist: {kubeconfigFileName}");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Could not use the provided kubeconfig file: {kubeconfigFileName}");
                logger.LogWarning($"Source: {ex.Source}. Error message: {ex.Message}");
                logger.LogWarning("Falling back to standard creation of Kubernetes Client.");
            }
        }

        if (k8sClient is null) // kubeconfigFileName is not IsNullOrWhiteSpace OR something bad happened
        {
            KubernetesClientConfiguration? defaultConfig = KubernetesClientConfiguration.IsInCluster()
                    ? KubernetesClientConfiguration.InClusterConfig()
                    : KubernetesClientConfiguration.BuildConfigFromConfigFile();
            k8sClient = new Kubernetes(defaultConfig, new PolicyHttpMessageHandler(GetRetryPolicy()));
        }
    }

    public Kubernetes GetClient() => k8sClient;

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
