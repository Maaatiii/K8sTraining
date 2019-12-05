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
