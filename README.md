# XAF-Blazor-Kubernetes-example
This example shows how to deploy XAF Blazor application to the Kubernetes cluster and to enable horizontal autoscaling. You can find here `Dockerfile` for publishing an app to the Linux container, and *.yaml files, describing deploying app to Kubernetes cluster with MSSQL database engine container, Horizontal Pod Autoscaler and Ingress.

## Getting started
1. Install Docker Engine or Docker Desktop

2. Build a docker image:
```
docker build -t <your_docker_hub_id>/xafcontainerexample --build-arg DX_NUGET_SOURCE=<your_devexpress_nuget_source_url> .
```

If you do this on Linux machine and see an error like “docker: Got permission denied while trying to connect to the Docker daemon socket at …”, just run the command as a superuser:
```
sudo docker docker build -t <your_docker_hub_id>/xafcontainerexample --build-arg DX_NUGET_SOURCE=<your_devexpress_nuget_source_url> .
```

You can run a container with this image by the following command:

```
docker run --publish <your_docker_hub_id>/xafcontainerexample:latest .
```

Now, your application is accessible by the `http://localhost/` URL. If you see a database version mismatch error in the console, force the db update by the launching another one application instance in the running container. Find the container's id by the `docker ps` command. Then, run the following:

```
docker exec <your_container_id> dotnet xafcontainerexample.Blazor.Server.dll --updateDatabase --forceUpdate --silent
```

In this particular solution example, we need to pass a CONNECTION_STRING environment variable specifiying which connection string (specified in the appsetting.json file) we will use in the container. Here the MySqlConnectionString allows using a database hosted on the host machine.

```
docker run --network="host" -e CONNECTION_STRING=MySqlConnectionString <your_docker_hub_id>/xafcontainerexample:latest .
```

4. (Optional) Here is an example how to run multi-container application using [Docker Compose](https://docs.docker.com/compose/) tool. The `docker-compose.yml` file describes the way the application and the MSSQL Server containers interact. Run the following command to launch:

```
docker compose up
```
Again, make sure that the app is running by the `http://localhost/` URL.

5. We need to store the image in Docker Hub. Log in with your docker credentials and push the image to your Docker Hub:

```
docker login
```
```
docker push <your_docker_hub_id>/xafcontainerexample:latest
```

5. Next, run a terminal on the machine with installed Kubernetes distribution. It can be Azure Kubernetes Service (AKS), Google Kubernetes Engine (GKE), locally installed lightweight k3s, minikube or some another version. This example was checked with AKS and locally installed k3s lightweight Kubernetes.

6. Apply a [Persistent Volume Claim](https://kubernetes.io/docs/concepts/storage/persistent-volumes/) definition to create a storage for the database:

```
kubectl apply -f ./K8S/local-pvc.yaml
```

7. Create a MSSQL Server deployment with its ClusterIP Service. But before that, we need to store the DB password into a secret:

```
kubectl create secret generic mssql --from-literal=SA_PASSWORD="Qwert1_"
```

```
kubectl apply -f ./K8S/mssql-app-depl.yaml
```

8. Create an application deployment with its ClusterIP Service:

```
kubectl apply -f ./K8S/app-depl.yaml
```

9. Now, we need to configure [Ingress](https://kubernetes.io/docs/concepts/services-networking/ingress/) to make the application deployment be accessible outside the cluster and to set up [Sticky Sessions](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server?view=aspnetcore-6.0#kubernetes). First, install ingress nginx controller if need (for example, for k3s: https://docs.rancherdesktop.io/how-to-guides/setup-NGINX-Ingress-Controller/). Next, apply the ingress definition:

```
kubectl apply -f ./K8S/ingress-srv.yaml
```

Wait for a couple of minute and check that the application is accessible outside the cluster:

```
kubectl get ingress
```

> NAME          CLASS   HOSTS   ADDRESS       PORTS   AGE
> ingress-srv   nginx   *       <your-ip>     80      5d21h


Try to open a starting page in the browser: http://<your-ip>/

10. The application is running just in a single [Pod](https://kubernetes.io/docs/concepts/workloads/pods/). To make it scale horizontally, let's use [Horizontal Pod Autoscaler](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/):

```
kubectl apply -f ./K8S/app-hpa.yaml
```

11. Now, you can check how many pods are running at this moment and their CPU load by the following command:

```
kubectl get hpa
```

> user@ubuntu-k8s:~/Work/xaf-blazor-app-load-testing-example$ kubectl get hpa
> NAME      REFERENCE             TARGETS   MINPODS   MAXPODS   REPLICAS   AGE
> app-hpa   Deployment/app-depl   13%/50%   1         15        7          54m


## Description
### Building a Docker Image



