## Storage

#### Changes in application, publishing new version
1. Open directory 2_storage
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
				claimName: azure-file
	```
	It's describe that our deployment need Persistent Volume Claim with name (azure-file) (https://kubernetes.io/docs/concepts/storage/persistent-volumes/)
	And it should be mounted in specific path **/usr/db**

2. Open K8sDeploymentScripts\sqllite-claim.yaml. It's definition of our PVC storage.
3. To install claim execute commands:

	```
	kubectl apply -f deployment.yaml
	```

	```
	kubectl apply -f azure-file-claim.yaml
	```
	
4. Open updated application and verify whether employee storage module is working.