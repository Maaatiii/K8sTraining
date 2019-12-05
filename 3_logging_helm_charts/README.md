## Add Logging support

#### Install seq 
Seq is the intelligent search, analysis, and alerting server built specifically for modern structured log data.

1. Execute command to install seq on your cluster
	```
	helm install stable/seq
	```
	After execution of this command you will have working seq server instance.
	IMPORTANT. Copy printed-out address of seq collector.

	```
	NOTES:
	Seq can be accessed via port 5341 on the following DNS name from within your cluster:
	jazzed-seastar-seq.testuser.svc.cluster.local
	```

2. To expose it to the external users you will need setup load balancer
3. Execute:
   ```
   kubectl get svc
   ```
   ```
   kubectl expose deployment halting-olm-seq --type=LoadBalancer --name=seq-service
   ```
   Where ``halting-olm-seq`` is the name of deployment. Hint you can check deployment name by ``kubectl get deployments``
4. Execute `` kubectl get svc`` and locate seq-service and copy port forwarded to 80 port
5. Open address **EXTERNAL-IP** , seq server should be opened.

#### Deploy application with logging support
1. Open 3_logging_helm_charts
2. Verify changes in application that was implemented to support logging:
	* Program.cs 	
		* Serilog logger has beend added with configuration of seq collector):
	
			```
			.WriteTo.Seq(Environment.GetEnvironmentVariable("SeqCollectorAddress"))
			```
		* UseSerilog extension method is used to configure logging with asp .net core:
	
			```
				public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
					WebHost.CreateDefaultBuilder(args)
					.UseSerilog()
					.UseStartup<Startup>();
			```

	* EmployeeController.cs
	
4. Publish new version of app with tag **v1.4**
5. Open K8sDeploymentScripts/deployment.yaml and update image version to **1.4**
6. Update ``SeqCollectorAddress`` to "http://{ip address of clusterip seq service}:5341"
7. Execute ``kubectl apply -f deployment.yaml`` to install new version

#### Verify logging feature
1. Open application and employee module 
2. Add test employees
3. Open seq and verify if logs appear
