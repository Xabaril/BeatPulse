# Install Beatpulse-ui on Kubernetes

We provide a _helm chart_ for easy installation of beatpulse-ui on a Kubernetes cluster. Currently _chart_ is provided as source code only, it will be published soon.

>**Note** This is a _preview_ feature and can be some changes in the future. Any feedback is very valuable.

## Download and install Helm

You need [Helm](https://helm.sh/) installed in your system. Browse to [https://github.com/kubernetes/helm] and follow the instructions in the _Install_ section.

Once _helm_ is installed in your system, you need to init _Tiller_ on your cluster, by simply typing (from a terminal):

```
helm init --wait
```

You can check what version of _helm_ do you have on both clien and server (cluster) running `helm version`. Output should be something like:

```
Client: &version.Version{SemVer:"v2.9.0", GitCommit:"f6025bb9ee7daf9fee0026541c90a6f557a3e0bc", GitTreeState:"clean"}
Server: &version.Version{SemVer:"v2.9.0", GitCommit:"f6025bb9ee7daf9fee0026541c90a6f557a3e0bc", GitTreeState:"clean"}
```

## Install the beatpulse-ui package

The folder `./beatpulse-ui` contains the chart source code. If you are not interested about how the chart is developed, you don't need to care about its contents.

For installing the chart you need to supply a YAML file containing all the URLs you want to check using beatpulse-ui. The format of file should be like:

```
healthchecks:
  - name: helloworld
    uri: http://helloworld.default
```

You can enter as many elements (name, uri) you need. The `uri` is the uri to check. It is internal to k8s server, so you can use any service name.

To install the chart just type:

```
helm install .\beatpulse-ui\ --values <your-YAML-file>
```

The chart installs following elements on your cluster:

1. One namespace called `beatpulse-ui`
2. One deployment named `beatpulse-ui` on this namespace
3. One service named `beatpulse-ui` on this namespace

## Hello world sample

We include two files:

* `sample.yaml`
* `values-sample.yaml`

First file deploys a simple helloworld running as a `NodePort` service. The second file is a YAML configuration for the chart to check this service. This sample is ready **to run using MiniKube**.

To run it:

1. Apply `sample.yaml` using `kubectl apply -f sample.yaml`
2. Install beatpulse-ui typing `helm install .\beatpulse-ui\ --values .\values-sample.yml`

Once installed check that `helloworld` service is running:

```
λ  kubectl get svc
NAME         TYPE        CLUSTER-IP       EXTERNAL-IP   PORT(S)        AGE
helloworld   NodePort    10.102.171.147   <none>        80:31511/TCP   14h
kubernetes   ClusterIP   10.96.0.1        <none>        443/TCP        33d
```

Check also that beatpulse-ui is running (note that your service name could be different):

```
λ  kubectl get svc -n beatpulse-ui
NAME                               TYPE       CLUSTER-IP      EXTERNAL-IP   PORT(S)        AGE
exacerbated-vulture-beatpulse-ui   NodePort   10.101.22.155   <none>        80:30850/TCP   18m
```

Now launch beatpulse-ui typing: 

```
minikube service <your-service-name> -n beatpulse-ui
``` 

>**Note**: You must add `/beatpulse-ui` to the URI. This will be solved in following versions. And you should see beatpulse-ui testing helloworld pod:

![./beatpulse-ui testing helloworld](./beatpulse-ui-ok.png)