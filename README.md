# XAF-Blazor-Kubernetes-example
This example shows how to deploy XAF Blazor application to the Kubernetes cluster and to enable horizontal autoscaling. You can find here `Dockerfile` for publishing an app to the Linux container, and *.yaml files, describing deploying app to Kubernetes cluster with MSSQL database engine container, Horizontal Pod Autoscaler and Ingress.

## Getting started
1. Install Docker Engine or Docker Desktop

2. Build a docker image:
```
docker build -t <your_docker_hub_id>/xafloadtesting --build-arg DX_NUGET_SOURCE=<your_devexpress_nuget_source_url> .
```
You can run a container with this image by the following command:

```
docker run <your_docker_hub_id>/xafloadtesting:latest .
```

Open 