# K8sTraining
![](https://upload.wikimedia.org/wikipedia/en/0/00/Kubernetes_%28container_engine%29.png)
## Environment configuration

#### Prerequisites:
* Docker installed
* Chocolatey
* Minikube
* Kubectl
* https://hub.docker.com/ account created
* ASP Net Core SDK
* Visual Studio 2019 with ASP .NET Core SDK (to build and publish your app) 

#### Chocolatey
Install chocolatey https://chocolatey.org/docs/installation
This package manager will be usefull to install other prerequisites.

#### Kubectl
```
choco install kubernetes-cli
```

#### Minikube
1. Install minikube version 1.3.0
	```
	choco install minikube --version 1.3.0
	```
2. Start minikube
	```
	minikube start --extra-config=apiserver.service-node-port-range=80-33300 --vm-driver=hyperv --hyperv-virtual-switch "{Name of switch}" --cpus 4 --memory 8192
	```
#### Helm charts
```
choco install kubernetes-helm --version 2.14.3

helm init
```

#### Docker
Download and install docker https://www.docker.com/get-started

---

And 

Let's start :-)

---

## Create sample solution and prepare application docker image

#### Checkout application from GitHub
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
3. Enter your credentials
4. Select **Edit Image Tag**>**CustomTag** option, and enter v1.0
5. Application will be published
6. You can check if repository has been created https://cloud.docker.com/u/{username}/repository/list

## Deploy the application

1. Checkout tag
	```
	git checkout 2_deployments_scripts
	```
2. Open directory K8sTraining\K8sDeploymentScripts (It will be usefull to use Visual Studio Code to edit yaml scripts)
3. Open **deployment.yaml**
4. Update image value to your image deployed in previous step
5. Execute kubectl command:
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
3. Execute command to list services
	```
	Kubectl get svc
	```
4. Copy second port i.e.  8080:4971/TCP it will be 4971
5. Execute and copy ip address from output.
	```
	minikube ip 
	```
6. Open in the browser - http://{minikube ip}:{port}

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

## Add storage

#### Changes in application, publishing new version
1. Checkout tag ```git checkout 3_sqllite_storage```
2. Employee managment module has been addeed to the website
	* Open file **EmployeeController.cs**
	* Open **Startup.cs**
3. Start application from visual studio, open employee module and try to add some records


4. Publish new version of app by click Solution->Publish, click 'Edit Image Tag' and enter **v1.3**

#### Deploy application with storage module
1. Open K8sDeploymentScripts\deployment.yaml - update image to published image
	Noticed that 2 things has been added to deployment definition:
	```
			volumeMounts:               
			- name: sqlite-volume       
			mountPath: /usr/db
			volumes:
			- name: sqlite-volume         
			persistentVolumeClaim:
				claimName: sqlite-pvc    
	```
	It's describe that our deployment need Persistent Volume Claim with name (sqllite-pvc) (https://kubernetes.io/docs/concepts/storage/persistent-volumes/)
	And it should be mounted in specific path **/usr/db**

2. Open K8sDeploymentScripts\sqllite-claim.yaml. It's definition of our PVC storage.
3. To install claim execute commands:

	```
	kubectl apply -f deployment.yaml
	```

	```
	kubectl apply -f sqllite-claim.yaml
	```
	
4. Open updated application and verify whether employee storage module is working.

## Add Logging support

#### Install seq 
Seq is the intelligent search, analysis, and alerting server built specifically for modern structured log data.

1. Execute command to install seq on your cluster
	```
	helm install stable/seq
	```
	After execution of this command you will have working seq server instance.
	IMPORTANT. Copy printed-out address of seq collector. 
2. To expose it to the external users you will need setup load balancer
3. Execute:
   ```
   kubectl get svc
   ```
   ```
   kubectl expose deployment halting-olm-seq --type=LoadBalancer --name=seq-service
   ```
4. Execute `` kubectl get svc`` and locate seq-service and copy port forwarded to 80 port
5. Open address **{minikube_ip}:{port}** , seq server should be opened.

#### Deploy application with logging support
1. Checkout tag ```git checkout 4_logging```
2. Verify changes in application that was implemented to support logging:
	* Program.cs 	
		* Serilog logger has beend added with configuration of seq collector):
	
			```
			.WriteTo.Seq("http://halting-olm-seq.default.svc.cluster.local:5341")
			```
		* UseSerilog extension method is used to configure logging with asp .net core:
	
			```
				public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
					WebHost.CreateDefaultBuilder(args)
					.UseSerilog()
					.UseStartup<Startup>();
			```

	* EmployeeController.cs
	
	
3. Update seq collector address in code with adress printed out after installation of helm chart.
4. Publish new version of app with tag **v1.4**
5. Open K8sDeploymentScripts/deployment.yaml and update image version to **1.4**
6. Execute ``kubectl apply -f deployment.yaml`` to install new version

#### Verify logging feature
1. Open application and employee module 
2. Add test employees
3. Open seq and verify if logs appear

## Helm charts to deploy app
To deploy our application we can use helm charts. 
Helm helps you manage Kubernetes applications — Helm Charts help you define, install, and upgrade even the most complex Kubernetes application.
A chart is a collection of files that describe a related set of Kubernetes resources. A single chart might be used to deploy something simple, like a memcached pod,or something complex, like a full web app stack with HTTP servers, databases, caches, and so on.

![](https://d1qy7qyune0vt1.cloudfront.net/nutanix-us/attachment/fa2af93e-44da-4ba4-a8b4-e39215f61a03.png)

#### Create new empty chart
1.  Open application main directory and create HELM folder, navigate to this folder
2.  To create helm chart execute:
	```
	helm create testwebapp
	```
3. Open values.yaml file
4. Locate **image** section
5. Change repository to your image  **docker.io/{user_name}/testwebapplication**
6. Update image tag to early version of your app **v1.1**

#### Deploy your chart
1. Execute in main directory of your chart
   ``` 
   helm install testwebapp
   ```
2. Verify if pod has been installed ``kubectl get pods``
3. Please notice that after installing helm chart there should be info presented
   ```
	NOTES:
		Get the application URL by running these commands:
		export POD_NAME=$(kubectl get pods --namespace default -l "app.kubernetes.io/name=testwebapp,app.kubernetes.io/instance=early-jackal" -o jsonpath="{.items[0].metadata.name}")
		echo "Visit http://127.0.0.1:8080 to use your application"
		kubectl port-forward $POD_NAME 8080:80
	```
4. Execute this steps and open url with app
5. Verify if app is working
   
## Ingress
In Kubernetes, an Ingress is an object that allows access to your Kubernetes services from outside the Kubernetes cluster. You configure access by creating a collection of rules that define which inbound connections reach which services. It's kind of reverse proxy.
(https://matthewpalmer.net/kubernetes-app-developer/articles/kubernetes-ingress-guide-nginx-example.html)

![](https://miro.medium.com/max/1294/1*RX1ZjiDaXIChc2b_5OYIww.png)

#### Enable Ingress on minikube
1. Check if ingress is enabled ``minikube addons list``
2. Execute ``minikube addons enable ingress`` to enable ingress
3. Verify if ingress controller is running ``kubectl get pods -n kube-system``

#### Enable ingress for helm chart
1. Open **values.yaml** of helm chart
2. Locate **ingress** section
3. Enable by set ``enabled: true``
4. Set host to ``host: testwebapp.{minikube_ip}.nip.io`` (https://nip.io/ dns service)
   and add empty path
	```
	path:
		/
	```
	
	Example:
	```
	hosts:
    - host: webapp.172.17.68.218.nip.io
      paths:
      - /
	```

#### Deploy app with Ingress
1. Navigate to helm directory and execute ``helm install testwebapp``
2. Execute ``kubectl get ingress`` to verify if ingress for our app has been installed
3. Open http://webapp.172.17.68.218.nip.io

##Materials:
   * https://medium.com/google-cloud/kubernetes-nodeport-vs-loadbalancer-vs-ingress-when-should-i-use-what-922f010849e0
   * **TBD**



## Usefull commands
* ``helm del $(helm ls --all --short) --purge``  remove all helm charts
* **TBD**