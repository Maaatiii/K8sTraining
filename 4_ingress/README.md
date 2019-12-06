## Ingress
In Kubernetes, an Ingress is an object that allows access to your Kubernetes services from outside the Kubernetes cluster. You configure access by creating a collection of rules that define which inbound connections reach which services. It's kind of reverse proxy.
(https://matthewpalmer.net/kubernetes-app-developer/articles/kubernetes-ingress-guide-nginx-example.html)

![](https://miro.medium.com/max/1294/1*RX1ZjiDaXIChc2b_5OYIww.png)

#### Install Ingress 

* Open working directory 4_ingress 

* Execute command 
``` 
helm install stable/nginx-ingress --generate-name
```
As a result you will have a ingress controller installed on your namespace

* Check address of ingress loadbalancer endpoitn
```
kubectl get svc
```
* Locate nginx-ingress-controller and copy **EXTERNAL-IP** address
* Open in the browser  

#### Expose your service by registering ingress rules

##### Install some webapps

1.  Add repo with test apps 
```
helm repo add azure-samples https://azure-samples.github.io/helm-charts/
```

4. Install 2 hello world apps from helm charts
```
helm install azure-samples/aks-helloworld --generate-name
```

```
helm install azure-samples/aks-helloworld --set title="AKS Ingress Demo" --set serviceName="ingress-demo" --generate-name
```

5. Create route to our applications, open directory K8sDeploymentScripts and open file **ingress_aks.yaml** update ip address according to comment

6. Apply this ingress rules by executing:

```
kubectl apply -f ingress_aks.yaml
```

7. Open in web browser:
   http://{ingress host}/hello-world-two
   http://{ingress host}/


























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
