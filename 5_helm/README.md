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
6. In file Chart.yaml update version your app - appVersion field to **v1.1** (simple hello world page from section 1) 

#### Deploy your chart
1. Execute in main directory of your chart
   ``` 
   helm install helmtestwebapp testwebapp
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

#### Expose using Ingress

* Update values.yaml:

```
ingress:
  enabled: true (1)
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /$2 (2)
  hosts:
    - host: test.51.105.212.200.nip.io (3)
      paths:
      - /test2(/|$)(.*) (4)
```
**Description:**
1. We need to enable ingress flag 
2. Add anotation to rewrite target (https://kubernetes.github.io/ingress-nginx/user-guide/nginx-configuration/annotations/#rewrite)
3. Set host using nip.io, please set you ingress loadbalancer ip address (we dont have dns so it's workaround to use our loadbalancer ip address)
4. This is path to our service
   

---
* Execute helm upgrade
```
helm upgrade helmtestwebapp testwebapp
```
* Open address http://test.51.105.212.200.nip.io/test2


## Customize helm chart
To customize helm chart we'll try to add secret to our chart
1. Open testwebapp/templates 
2. Create new file secret.yaml
```
apiVersion: v1
kind: Secret
metadata:
  name: {{ include "testwebapp.fullname" . }}-secret
  labels:
{{ include "testwebapp.labels" . | indent 4 }}
type: Opaque
stringData:
  username: admin
  password: Haslo123
```
3. Execute helm upgrade
```
helm upgrade helmtestwebapp testwebapp
```
4. Verify whether secret has been added
```
kubectl get secrets
```

But we want to customize data for this secret in values.yaml file not hardcode in templates:

1. Update Secret.yaml
```
stringData:
  username: {{ .Values.cred.username }}
  password: {{ .Values.cred.pass }}
```

2. We need to define new values in values.yaml, open this file and add following section

```
cred:
  username: "adminadmin"
  pass: "Test1234!"
```

3. Execute upgrade and verify if it has succeed.