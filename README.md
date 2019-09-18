# K8sTraining

## Environment configuration

#### Prerequisites:
* Docker installed
* Chocolatey 
* Minikube
* Kubectl
* https://hub.docker.com/ account created
* Asp Net Core
* Visual Studio Code
* VS 2019 

#### Chocolatey
Install chocolatey https://chocolatey.org/docs/installation

#### Kubectl
```
choco install kubernetes-cli
```
#### Minikube
1. Install minikube
```
choco install minikube
```
2. Start minikube
```
minikube start --extra-config=apiserver.service-node-port-range=80-33300 --vm-driver=hyperv --hyperv-virtual-switch "Name of switch"
```
#### Helm charts
```
choco install kubernetes-helm

helm init
```

#### Docker
Download and install docker https://www.docker.com/get-started

## Create sample solution and prepare application docker image

#### Checkout application from github
1. Open https://github.com/Maaatiii/K8sTraining
2. Clone repository
```
git clone https://github.com/Maaatiii/K8sTraining.git
```
	
#### Create docker image and publish to dockerhub
1. Checkout tag
```
git checkout 1_sample_application
```
2. Navigate to Solution->Publish->Container Registry->Docker Hub and click **Publish**
3. Enter credentials to docker hub
4. Application will be published
5. You can check if repository has been created https://cloud.docker.com/u/{username}/repository/list

## Deploy the application

1. Checkout tag
```
git checkout 2_deployments_scripts
```
2. Open directory K8sTraining\K8sDeploymentScripts with Visual Studio Code (**hint** open this directory from cmd and enter code .)
3. Open deployment.yaml
4. Update image value to your image deployed in previous step
5. Execute kubectl command:
```
kubectl apply -f deployment.yaml
```
6. Check if there is any pod deployed
```
kubectl get pods
```
#### Access using port-forward
1. Execute 
```
Kubectl port-forward pods/{name of your pod} 8880:80
```
2. Open http://localhost:8880/

#### Expose application by Load Balancer service
1. Open service.yaml in visual studio code
2. Execute kubectl command:
```
kubectl apply -f service.yaml
```
3. Execute command to list services
```
Kubectl get svc
```
4. Copy second port i.e.  8080:4971/TCP it will be 4971
5. Execute 
```
minikube ip 
```
and copy ip address
6. Open address http://{minikube ip}:{port}

### Update application, rollout new version and rollback

#### Update to new version of application
1. Change something in mvc project (modify home page)
2. Publish new version of app by click Solution->Publish, click 'Edit Image Tag' and enter **v1.1**
3. After image has been published execute
```
kubectl set image deployment/testwebapp testwebapp=docker.io/{user}/testwebapplication:v1.1
```
4. Execute 
```
kubectl get pods
```
verify if pods has been updated 
5. Execute command to verify status of rollout
```
kubectl rollout status deployments/testwebapp
```
6. Open address http://{minikube ip}:{port} and verify if changes has been deployed

#### Update to not exist version of application
1. Try to update to not existing version of application
```
kubectl set image deployment/testwebapp testwebapp=docker.io/{user}/testwebapplication:v1.2
```
2. Execute 
```
kubectl get pods
```
3. Open application page (app is not working) 

#### Check detailed status of pod
1. Get detailed state of pod
```
kubectl logs pods/{name of pod}
```

### Add storage to our app

### Add Logging support

### Secrets 

### Helm charts to deploy app

### Ingress

## Ingress
