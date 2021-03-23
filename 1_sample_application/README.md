## Create sample solution and prepare application docker image

#### Checkout application from GitHub
1. Open https://github.com/Maaatiii/K8sTraining
2. Clone repository
	```
	git clone https://github.com/Maaatiii/K8sTraining.git
	```
	
#### Create docker image and publish to dockerhub
1. Open folder ``1_sample_application``
2. Open solution ``TestWebApplication.sln``
3. Navigate to Solution->Publish->Container Registry->Docker Hub and click **Publish**
4. Enter your credentials
5. Select **Edit Image Tag**>**CustomTag** option, and enter ``v1.0``
6. Application will be published
7. You can check if repository has been created https://cloud.docker.com/u/{username}/repository/list

## Deploy the application
1. Open directory K8sDeploymentScripts (It will be usefull to use Visual Studio Code to edit yaml scripts)
2. Open **deployment.yaml**
3. Update image value to your image deployed in previous step
4. Execute kubectl command:
	```
	kubectl apply -f deployment.yaml
	```
	to to add our deployment.
6. Check if there is any pod deployed
	```
	kubectl get pods
	```
#### Access using port-forward
1. Execute 
	```
	kubectl port-forward pods/{name of your pod} 8880:80
	```
2. Open http://localhost:8880/

#### Expose application by Load Balancer service
1. Open K8sDeploymentScripts\service.yaml in visual studio code
2. Execute kubectl command:
	```
	kubectl apply -f service.yaml
	```
2. Open new terminal and execute (to create tunnel for the loadbalancer)
	```
	minikube tunnel
	```
4. Execute command to list services
	```
	kubectl get svc
	```
4. Copy ip address - EXTERNAL-IP - of load balancer , if it's pending wait  and execute this command again
5. Open in the browser - http://{EXTERNAL-IP}

## Update application, rollout new version and rollback
#### Update to new version of application
1. Change something in mvc project (i.e. modify home page add some text)
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
6. Open address http://{EXTERNAL-IP} and verify if changes has been deployed

#### Update to not exist version of application
1. Try to update to not existing version of application
	```
	kubectl set image deployment/testwebapp testwebapp=docker.io/{user}/testwebapplication:notexistingtag
	```
2. Execute 
	```
	kubectl get pods
	```
3. Open application page (app isn't working) 

#### Check detailed status of pod
1. Get detailed state of pod
	```
	kubectl describe pod {name of pod}
	```

2. Get logs from pod
	```
	kubectl logs pods/{name of pod}
	```

#### Rollback 
1. Rollback application update by executing
	```
	kubectl rollout undo deployments/testwebapp
	```
2. Open application page


## Scale 

1. Modify code HomeController.cs to print out machine name 
```
ViewData["MachineName"] = Environment.MachineName;
```

in Index.cshtml
```
<h4>@ViewData["MachineName"]</h4>
```
2. Publish new version of application to the docker repository and then update deployment to new version.

3. Open application in the browser refresh and notice there is one machine name printed-out.
Because we configured only one  **replicas: 1** check in **deployment.yaml**

4. Open **deployment.yaml** and modify replicas to 2
5. Execute
```
kubectl apply -f deployment.yaml
```
6. Execute 
``` 
kubectl get pods
```
And notice that number of pods increased 

7. Open browser and refresh application page, notice that machine name is changing

## Liveness probe

Kubernetes can check if a container is still alive through liveness probes. You can specify
a liveness probe for each container in the pod’s specification. Kubernetes will periodically
execute the probe and restart the container if the probe fails.

1. Modify code of asp project, open Startup.cs and add 
* in ConfigureServices method:
```
services.AddHealthChecks();
```
* in Configure
```
app.UseHealthChecks("/health");
```

2. Publish our app and deploy to the cluster 
3. Verify if app is working
4. Modify deployment.yaml file
```
      containers:
        - name: testwebapp
          image: docker.io/mcywka/testwebapplication:test_1.5
          ports:
            - containerPort: 80
          env:
            - name: Hello
              value: "Hello from docker"
          livenessProbe:
            httpGet:
              path: /health
              port: 80
            initialDelaySeconds: 3
            periodSeconds: 3
```
and add livenessProbe section
5. Update deployment
```
kubectl apply -f deployment.yaml
```
6. Open app and verify if it's working
7. Open application page /health to verify healthcheck


## TODO
* Env variables, update env variable and check 
* Liveness probe based on file (delete file from container and check)
