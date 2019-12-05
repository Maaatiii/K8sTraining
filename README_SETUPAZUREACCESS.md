
## Connect to azure cluster

```
az login 

az account set --subscription "d0db0c86-7c31-4614-99c4-6b39dfd3a98e"
```

Get credentials from created cluster

```
az aks get-credentials --resource-group rose-k8s --name rose-k8s-play
```

Check if you are able to connect to the cluster by execute simple kubectl command:

```
kubectl get pods
```

## Create your namespace

```
kubectl create namespace {your user}
```

And set your name as a current namespace

```
kubectl config set-context --current --namespace={your user}
```
