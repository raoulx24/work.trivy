# Based on https://gist.github.com/sdarwin/ffc5fed82b6128a7549fd6d5696d98a8
# Based on https://gist.github.com/innovia/fbba8259042f71db98ea8d4ad19bd708

# this powershell script is used to create custom kubeconfig based on the provided k8s Service Account (SA) name, its namespace and
# kubeconfig out file name.
# They sould already exist and will be retreived from the current config context
# Normally, after 1.24, $secretName shouldn't exists and you have to create it. And only for testing purposes.
# Please do read
# https://gist.github.com/innovia/fbba8259042f71db98ea8d4ad19bd708?permalink_comment_id=4501051#gistcomment-4501051

$serviceAccountName = ""  # the SA name. i.e. trivy-dashboard-sa
$namespace = ""           # namespace where SA exists. i.e. default
$kubecfgFileName = ""     # a file name. i.e. %TEMP%\custom.kube.config or /tmp/custom.kube.config
$secretName = "$serviceAccountName-secret" # it must exist. change it if you wish

$secretObj = (kubectl get secret --namespace $namespace $secretName -o json) | ConvertFrom-Json
$caCrt = [Text.Encoding]::Utf8.GetString([Convert]::FromBase64String($secretObj.data.'ca.crt'))
$token = [Text.Encoding]::Utf8.GetString([Convert]::FromBase64String($secretObj.data.token))

$kubecConfigObj = (kubectl config view -o json) | ConvertFrom-Json
$clusterName = $kubecConfigObj.contexts.Where({$_.name -eq $kubecConfigObj.'current-context'},'First').context.cluster
$endpoint = $kubecConfigObj.clusters.Where({$_.name -eq $clusterName},'First').cluster.server

$caCrtFile = [IO.Path]::Combine([System.IO.Path]::GetTempPath(), "ca.crt")
$caCrt | Out-File -FilePath $caCrtFile

kubectl config set-cluster "$clusterName" --kubeconfig="$kubecfgFileName" --server="$endpoint" --certificate-authority="$caCrtFile" --embed-certs=true

kubectl config set-credentials "$serviceAccountName-$namespace-$clusterName" --kubeconfig="$kubecfgFileName" --token="$token"

kubectl config set-context "$serviceAccountName-$namespace-$clusterName" --kubeconfig="$kubecfgFileName" --cluster="$clusterName" --user="$serviceAccountName-$namespace-$clusterName" --namespace="$namespace"

kubectl config use-context "$serviceAccountName-$namespace-$clusterName" --kubeconfig="$kubecfgFileName"