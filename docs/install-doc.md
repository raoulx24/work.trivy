# Installation and Configuration

## Prerequisites

To run, the app needs a Kubernetes cluster. If the app is started without any additional setup ("as is"), needed minimal RBAC rights are as follows

| apiGroup               | resource  | verbs            |
|------------------------|-----------|------------------|
|                        | namespace | get, watch, list |
| aquasecurity.github.io | *         | get, watch, list |

If, for any reason, the right watch on namespaces cannot be provided, then the ones for `apiGroup aquasecurity.github.io` are still required, and the value for parameter `namespaceList` must be provided (more info in [Specific Parameters](#specific-parameters)).

> **Note:** why watch on namespaces is needed: the app starts a watcher on namespaces as it needs to be aware of any changes, and to start (or stop) the subsequent watchers on newer (or deleted) namespaces accordingly

## Installation

The recommended way of installation is via helm. The files are provided in `deploy/helm`. The helm is a "standard" one (as obtained by `helm create` command and added specific values and files).

> **Note:** The file `deploy/static/trivy-operator-dashboard.yaml` is a render of the mentioned helm with default values.

Steps:

1. customize `values.yaml` file. The parameters from `# app related params` section are explained in [Specific Parameters](#specific-parameters)
2. if ingress with TLS is needed, update the value for `ingress.tls.secretName` and create the TLS secret. Example:
```sh
kubectl create secret tls chart-example-tls --cert=path/to/cert/file --key=path/to/key/file
```
3. run the helm. Example:
```sh
helm install trivy-operator-dashboard trivy-operator-dashboard
```

## Specific Parameters

In helm values file, the following parameters are app related:

| key name                            | description |
|-------------------------------------|-----|
| kubeConfigFileName                  | path to custom kube config file; normally needed only for dev stage. in k8s it should be empty |
| namespaceList                       | a list of namespaces, comma delimited. if provided, the namespaces watcher is disabled |
| trivyUseClusterRbacAssessmentReport | activate or deactivate Cluster RBAC Assessment Report module; if false, the watchers are disabled |
| trivyUseConfigAuditReport           | activate or deactivate Config Audit Report module; if false, the watchers are disabled |
| trivyUseExposedSecretReport         | activate or deactivate Exposed Secret Report module; if false, the watchers are disabled |
| trivyUseVulnerabilityReport         | activate or deactivate Vulnerability Report module; if false, the watchers are disabled |

> **Note:** the above described parameters have correspondence in appsettings.json. That file is for dev purposes

## Considerations

### Resources (Requests/Limits)

The app utilizes caching to deliver fast responses. By storing all data in memory, it significantly reduces the need for repetitive Kubernetes API queries, thereby enhancing performance and minimizing latency, without significant memory overhead. Even though the provided (and commented) resources values are more than enough for some hundreds of scaned images (educated guess is that 500 is a safe number), please do adjust the values based on your needs.

### Running the App

Although there are other means of running the app, such as a "thick client" on a desktop OS, split in frontend and backend, scaled, even in docker (if you insist), they are not in the scope of this document.

### Kubernetes RBAC

In the Kubernetes cluster, there are some other ways of combining RBAC rights. For instance, instead of cluster roles, simple namespaced roles can be created. This is a more restricted way of running the app and is pertinent to "multi-tenant clusters" (where same cluster is shared by distinct groups). Also, they are not in the scope of this document.

### Logging - Serilog

The logging component of the backend is based on [Serilog](https://github.com/serilog/serilog/blob/dev/README.md). The file sink can be activated by using `extraEnvValues` from `values.yaml` file, like this:
```yaml
extraEnvValues:
- name: SERILOG__WRITETO__1__NAME
  value: "File"
```
Any other Serilog related parameters can be modified in the same way.

> **Note:** writing directly to container storage without utilizing volumes is strongly discouraged for several critical reasons, including data persistence, security, and resource management. To activate this feature safely and effectively, it is essential to attach a volume to the pod; this is not in the scope of this document

Related to Serilog sinks, only Console and File are present at runtime. If other ones are needed, you can do a custom build of the app or provide them in the image or in the container (via configmap, or init container) and add the needed environment variables in `extraEnvValues` from `values.yaml` file. Also, they are not in the scope of this document.
