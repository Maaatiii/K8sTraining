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
