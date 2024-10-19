using k8s;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Infrastructure.Clients;

public class KubernetesClientFactory : IKubernetesClientFactory
{
    private readonly Kubernetes kubernetesClient;
    private ILogger logger;

    static KubernetesClientFactory()
    {
        KubernetesJson.AddJsonOptions(ConfigureJsonSerializerOptions);
    }

    public KubernetesClientFactory(IOptions<KubernetesOptions> options, ILogger<KubernetesClientFactory> logger)
    {
        this.logger = logger;
        
        string? kubeConfigFileName = options.Value.KubeConfigFileName;
        if (!string.IsNullOrWhiteSpace(kubeConfigFileName))
        {
            try
            {
                if (File.Exists(kubeConfigFileName))
                {
                    KubernetesClientConfiguration config =
                        KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFileName);
                    config.AddJsonOptions(ConfigureJsonSerializerOptions);
                    kubernetesClient = new Kubernetes(config);
                }
                else
                {
                    logger.LogWarning(
                        "Provided kubeConfig file does not exist: {kubeConfigFileName}",
                        kubeConfigFileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    "Could not use the provided kubeConfig file: {kubeConfigFileName}",
                    kubeConfigFileName);
                logger.LogWarning(
                    ex,
                    "Source: {exceptionSource}. Error message: {exceptionMessage}",
                    ex.Source,
                    ex.Message);
                logger.LogWarning("Falling back to standard creation of Kubernetes Client.");
            }
        }

        if (kubernetesClient is null) // kubeConfigFileName is not IsNullOrWhiteSpace OR something bad happened
        {
            KubernetesClientConfiguration? defaultConfig = KubernetesClientConfiguration.IsInCluster()
                ? KubernetesClientConfiguration.InClusterConfig()
                : KubernetesClientConfiguration.BuildConfigFromConfigFile();
            defaultConfig.AddJsonOptions(ConfigureJsonSerializerOptions);
            kubernetesClient = new Kubernetes(defaultConfig);
        }
    }

    public Kubernetes GetClient() => kubernetesClient;

    private static void ConfigureJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        jsonSerializerOptions.Converters.Insert(0, new DateTimeJsonConverter());
        jsonSerializerOptions.Converters.Insert(0, new DateTimeNullableJsonConverter());
    }
}
