# XAF-Blazor-Kubernetes-example
This example shows how to deploy XAF Blazor application to the Kubernetes cluster and to enable horizontal autoscaling. You can find here `Dockerfile` for publishing an app to the Linux container, and *.yaml files, describing deploying app to Kubernetes cluster with MSSQL database engine container, Horizontal Pod Autoscaler and Ingress. 

The diagram below shows how our cluster looks:

![Cluster diagram](/images/cluster-diagram.png)

This application was tested with locally-running cluster (https://k3s.io/) and [Azure Kubernetes Service](https://azure.microsoft.com/en-us/services/kubernetes-service/). The maximum pod replicas number (20) allowed working for 300 concurrent users. The AKS cluster with two nodes (each machine like B4ms: 4 Cores, 16 GB RAM) also can operate with such number of pod replicas and such load.

## Getting started
1. Install Docker Engine or Docker Desktop

2. Clone this repository

3. Build a docker image:
```
docker build -t <your_docker_hub_id>/xafcontainerexample --build-arg DX_NUGET_SOURCE=<your_devexpress_nuget_source_url> .
```

If you do this on Linux machine and see an error like “docker: Got permission denied while trying to connect to the Docker daemon socket at …”, just run the command as a superuser:
```
sudo docker docker build -t <your_docker_hub_id>/xafcontainerexample --build-arg DX_NUGET_SOURCE=<your_devexpress_nuget_source_url> .
```

You can run a container with this image by the following command:

```
docker run <your_docker_hub_id>/xafcontainerexample:latest .
```

Now, your application is accessible by the `http://localhost/` URL. If you see a database version mismatch error in the console, force the db update by the launching another one application instance in the running container. Find the container's id by the `docker ps` command. Then, run the following:

```
docker exec <your_container_id> dotnet xafcontainerexample.Blazor.Server.dll --updateDatabase --forceUpdate --silent
```

In this particular solution example, we need to pass a CONNECTION_STRING environment variable specifiying which connection string (specified in the appsetting.json file) we will use in the container. Here the MySqlConnectionString allows using a database hosted on the host machine.

```
docker run --network="host" -e CONNECTION_STRING=MySqlConnectionString <your_docker_hub_id>/xafcontainerexample:latest .
```

4. We need to store the image in Docker Hub. Log in with your docker credentials and push the image to your Docker Hub:

```
docker login
```
```
docker push <your_docker_hub_id>/xafcontainerexample:latest
```

Note: change the image names in the `app-depl.yaml` and `docker-compose.yml` (optionally) files to pull already built docker images from your own Docker hub.

5. (Optional) Here is an example how to run multi-container application using [Docker Compose](https://docs.docker.com/compose/) tool. The `docker-compose.yml` file describes the way the application and the MSSQL Server containers interact. Run the following command to launch:

```
docker compose up
```
Again, make sure that the app is running by the `http://localhost/` URL.

6. Next, run a terminal on the machine with installed Kubernetes distribution. It can be Azure Kubernetes Service (AKS), Google Kubernetes Engine (GKE), locally installed lightweight k3s, minikube or some another version. This example was checked with AKS and locally installed k3s lightweight Kubernetes.

7. Apply a [Persistent Volume Claim](https://kubernetes.io/docs/concepts/storage/persistent-volumes/) definition to create a storage for the database:

```
kubectl apply -f ./K8S/local-pvc.yaml
```

8. Create a MSSQL Server deployment with its ClusterIP Service. But before that, we need to store the DB password into a secret:

```
kubectl create secret generic mssql --from-literal=SA_PASSWORD="Qwert1_"
```

Then, apply deployment manifest:
```
kubectl apply -f ./K8S/mssql-app-depl.yaml
```

9. Create an application deployment with its ClusterIP Service:

```
kubectl apply -f ./K8S/app-depl.yaml
```

Note: To update the database, you can use the similar approach as above. First, find a pod with running application:

```
kubectl get pods
```

```
NAME                         READY   STATUS    RESTARTS     AGE
app-depl-f487bdcfd-mxnrz     1/1     Running   0            75m
mssql-depl-c47fdc8c7-5x5m7   1/1     Running   1 (2s ago)   5s
```

For example, our application pod's name is `app-depl-f487bdcfd-mxnrz`. Then, run another one application instance in the db updating mode:

```
kubectl exec -it %pod_name% -- dotnet XAFContainerExample.Blazor.Server.dll --updateDatabase --forceUpdate --silent
```

10. Now, we need to configure [Ingress](https://kubernetes.io/docs/concepts/services-networking/ingress/) to make the application deployment be accessible outside the cluster and to set up [Sticky Sessions](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server?view=aspnetcore-6.0#kubernetes). First, install ingress nginx controller if need (for example, for k3s: https://docs.rancherdesktop.io/how-to-guides/setup-NGINX-Ingress-Controller/). Next, apply the ingress definition:

```
kubectl apply -f ./K8S/ingress-srv.yaml
```

Wait for a couple of minute and check that the application is accessible outside the cluster:

```
kubectl get ingress
```

```
NAME          CLASS   HOSTS   ADDRESS       PORTS   AGE
ingress-srv   nginx   *       <your-ip>     80      5d21h
```

Try to open a starting page in the browser: http://<your-ip>/

11. The application is running just in a single [Pod](https://kubernetes.io/docs/concepts/workloads/pods/). To make it scale horizontally, let's use [Horizontal Pod Autoscaler](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/):

```
kubectl apply -f ./K8S/app-hpa.yaml
```

12. Now, you can check how many pods are running at this moment and their CPU load by the following command:

```
kubectl get hpa
```

```
user@ubuntu-k8s:~/Work/xaf-blazor-app-load-testing-example$ kubectl get hpa
NAME      REFERENCE             TARGETS   MINPODS   MAXPODS   REPLICAS   AGE
app-hpa   Deployment/app-depl   13%/50%   1         15        7          54m
```

## Description

### Building XAF Blazor application Docker image

This solution contains a `Dockerfile` example based on the [microsoft-dotnet](https://hub.docker.com/_/microsoft-dotnet) images.

```
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG DX_NUGET_SOURCE
WORKDIR /src
RUN dotnet nuget add source $DX_NUGET_SOURCE -n devexpress-nuget
COPY ["XAFContainerExample.Blazor.Server/XAFContainerExample.Blazor.Server.csproj", "XAFContainerExample.Blazor.Server/"]
COPY ["XAFContainerExample.Module/XAFContainerExample.Module.csproj", "XAFContainerExample.Module/"]
RUN dotnet restore "XAFContainerExample.Blazor.Server/XAFContainerExample.Blazor.Server.csproj"
COPY . .
WORKDIR "/src/XAFContainerExample.Blazor.Server"
RUN dotnet build "XAFContainerExample.Blazor.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XAFContainerExample.Blazor.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XAFContainerExample.Blazor.Server.dll"]
```

You can also generate it using Visual Stuido Wizard: right click on the YourApp.Blazor.Server project, then choose “Add” -> “Docker Support”. Note that you need to move the created `Dockerfile` up to the root solution folder in this case.

![Docker support](/images/docker-support.png)


Also, to restore nuget packages correctly, we will pass the DevExpress nuget source url via argument ([ARG](https://docs.docker.com/engine/reference/builder/#arg)). At the end of the section, we will publish our application to the /app directory and copy the entrypoint.sh file there.

Refer to [Docker reference](https://docs.docker.com/engine/reference/builder/) for better understanding commands syntax.

Note: here you can find a `Dockerfile.win` intended for Windows container. To change the container type in the running Docker instance, right-click the System Tray's Docker icon and choose Switch to Windows containers... If you want to build an image with custom Dockerfile name, use the `-f ` flag:

```
docker build -f Dockerfile.win -t <your_docker_hub_id>/xafcontainerexample --build-arg DX_NUGET_SOURCE=<your_devexpress_nuget_source_url> .
```
### Running XAF Blazor application in Kubernetes cluster

This section describes all the specifications located in the `K8S` folder. They should be enough to deploy and run XAF Blazor application with load balancing and autoscaling.

1. Database deployment

The database deployment requires a storage. The PersistentVolume subsystem provides an API for users and administrators that abstracts details of how storage is provided from how it is consumed. A PersistentVolumeClaim (PVC) is a request for a storage. Here is a specification for a simple PVC ([local-pvc.yaml](/K8S/local-pvc.yaml)) which is pretty enough for locally-running cluster (e.g. [k3s](https://k3s.io/)):

```
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: mssql-claim
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 1Gi
```

The [mssql-app-depl.yaml](/K8S/mssql-app-depl.yaml) file contains specifications for database engine [deployment](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#writing-a-deployment-spec), which runs MSSQL Server in a container, and a ClusterIP service which is intended to [expose an app](https://kubernetes.io/docs/tutorials/kubernetes-basics/expose/expose-intro/) on an internal IP in the cluster. The db server will be only reachable inside the cluster.

2. Application deployment

The [app-depl.yaml](/K8S/app-depl.yaml) file describes application deployment itself and a ClusterIP service for it (similar to described above):

```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: app-depl
spec:
  replicas: 1
  selector:
    matchLabels:
      app: xafcontainerexample
  template:
    metadata:
      labels:
        app: xafcontainerexample
    spec:
      containers:
      - name: xafcontainerexample
        image: ostashev/xafcontainerexample:latest
        imagePullPolicy: Never
        env:
          - name: CONNECTION_STRING
            value: K8SMsSqlConnectionString
        resources:
          requests:
            cpu: 400m
            memory: 500Mi
          limits:
            cpu: 800m
            memory: 1Gi
```

Here we specify image which was built before, additional environment variables (e.g. CONNECTION_STRING) and what hardware resources the cluster should reserve for this container.

3. Ingress

Ingress exposes HTTP and HTTPS routes from outside the cluster to services within the cluster. Traffic routing is controlled by rules defined on the Ingress resource. Due to fact that Blazor Server application uses long-living Websocket to communicate between browser and server, we need to [Sticky Sessions](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server?view=aspnetcore-6.0#kubernetes) to keep the connection to the particular Pod all the time during application using. This [ingress definition](/K8S/ingress-srv.yaml) example is written for Kubernetes version 1.19 and later. Cluster must have an [ingress controller](https://kubernetes.io/docs/concepts/services-networking/ingress-controllers/) running.

4. Horizontal Pod Autoscaler.

The [app-hpa.yaml](/K8S/app-hpa.yaml) manifest defines a HorizontalPodAutoscaler (HPA) which scales the number of the running pod replicas according to the specified metricas.

```
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
 name: app-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: app-depl
  minReplicas: 1
  maxReplicas: 20
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 50
```
In this example, we can scale pod replicas from 1 (`minReplicas`) up to 20 (`maxReplicas`) according to the CPU utilization. Refer the [HPA](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/#algorithm-details) documentation to learn more.

### Running multi-container application with Docker compose

If you don't want to scale app automatically and set up Kubernetes, you may consider using Docker Compose tool. The `docker-compose.yml` file contains definitions for two containers. The first container uses `xafcontainerexample` image which was described previously. The second allows running MSSQL Server in another container and get access to it from the first one.

```
version: "3.9"
services:
    web:
        image: "ostashev/xafcontainerexample:latest"
        build:
            context: .
            dockerfile: "./Dockerfile"
            args:
              DX_NUGET_SOURCE: ${DX_NUGET_SOURCE}
        ports:
          - "80:80"
        environment:
            - CONNECTION_STRING=DockerMsSqlConnectionString
            
    db:
        image: "mcr.microsoft.com/mssql/server"
        environment:
            SA_PASSWORD: "Qwerty1_"
            ACCEPT_EULA: "Y"
        ports:
          - "1433:1433"
```

Now, the application running in the first contaier can access the database with the connection string like following:

```
Pooling=false;Data Source=db;Initial Catalog=XAFContainerExample;User Id=SA;Password=<your_strong_password>
```

Refer the [Compose specification](https://docs.docker.com/compose/compose-file/) topic for better compose file format understanding.

### Troubleshooting and limitations

1. When the HPA scales down replicas, some users see an "Connection Error" message in the browser.

This problem is caused by Sticky Session. The browser communicates only with one particular server all the time when page is opened. When the pod replica is being terminated, the connection is lost. A possible workaround for this case is to refresh the browser automatically when unable reconnect to the server. You can find an example of this approach here: [_Host.cshtml](/XAFContainerExample.Blazor.Server/Pages/_Host.cshtml). 

2. Ingress does not work on k3s Kubernetes distribution. The application web page cannot be reached outside the cluster.

Check the ingress-nginx-controller service:

```
kubectl get svc -n ingress-nginx
```

```
NAME                                 TYPE           CLUSTER-IP     EXTERNAL-IP     PORT(S)                      AGE
ingress-nginx-controller-admission   ClusterIP      10.43.168.32   <none>          443/TCP                      14m
ingress-nginx-controller             LoadBalancer   10.43.132.9    <pending>       80:31075/TCP,443:32734/TCP   14m
```
If you see that the `ingress-nginx-controller` service external IP is pending infinitely, probably you faced with this issue: https://github.com/rancher/k3os/issues/208. Try a workaround from this comment: https://github.com/rancher/k3os/issues/208#issuecomment-599087377

```
kubectl patch svc ingress-nginx-controller -n ingress-nginx -p '{"spec": {"type": "LoadBalancer", "externalIPs":["your-external-ip"]}}'
```
