# Introduction

This repo contains demos in `src`, and these presentation notes (below).

# Journey to Containers (in Azure)

### Introduction

- This presentation is a discussion starter. So talk about your journey!

- Microsoft used to promote three personas - mort, elvis, einstein. In reality, we should try to become an 'elvis' and leave the state-of-the-art to very large organizations
- So, we are not attempting to do 'platform engineering' or anything higher up the Maslow's (DevOps) Hierarchy of Needs
- we are small, we run everything: many websites, lots of bursty background processing - in the parlance of containers, running many Stateless Services

### Our Story

The year is 2020:

- Pain point: running in Cloud Services and Service Fabric
- Cloud Services: deprecated technology, slow deployments, inefficient Windows VMs
- Service Fabric: an inferior, Windows-based version of Kubernetes - harder to develop against, manage, deploy, scale, had a tiny ecosystem, meanwhile had all the costs and burden maintaining a Kubernetes cluster

1. docker-compose for local development
2. converted ancillary apps from .NET Framework to .NET Core, got feedback and experience
3. Kubernetes: set up a simple AI workload on GPU nodes
4. converted our .NET Framework monolith, over months, in pieces, to .NET Core

- converted ancillary apps from .NET Framework to .NET Core, got feedback and learned
- converted our .NET Framework monolith slowly, over months, in pieces, to .NET Core. Simultaneous with the Framework -> Core migration was a migration to containers. The last thing to move: flagship (most complex) website - used strangler pattern for background workloads (message queues help!)
- we're done! now to live in Kubernetes, for better or worse

#### Upgrading from .NET Framework to dotnet 7 is difficult, but doable

- Use every tool at your disposal:
  - https://aws.amazon.com/porting-assistant-dotnet/
  - https://learn.microsoft.com/en-us/dotnet/core/porting/#tools-to-assist-porting
  - https://www.jetbrains.com/help/resharper/Navigation_and_Search__Finding_Usages__Optimizing_References.html
- Migrate in steps:

  1. NuGet cleanup: https://learn.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
     - note that testing NuGet changes will require exercising the code/loading the .NET assembly at runtime--compiling is not sufficient.
  2. Migration viability check - will everything move? Helpful tool: 🧨 NU1701 warning suppression 🧨 - allows you to dangerously reference .NET Framework dlls from dotnet 7 - https://stackoverflow.com/a/44999938
  3. Migrate to SDK-style project files: https://codeopinion.com/migrating-to-sdk-csproj/ - lean on `git blame` to see if you have made any 👻 spooky changes 👻 to the csproj
  4. Migrate all shared libraries to .NET Standard
  5. Migrate test projects to dotnet 7
  6. Migrate remaining projects to dotnet 7, including a full ASP.NET MVC -> aspnetcore migration

- major immediate cost savings: Windows -> Linux
- major immediate cost savings: leveraging spot VMs (~50-90% discount)
- major cost savings, after gaining confidence in scaling metrics: scale-to-zero

- processing time improvement: faster autoscaling
- minor: deployments are 3x-5x faster, and less flaky
- improved monitoring
- better vulnerability scanning
- 🚫🚫 we have had to bootstrap all Kubernetes ecosystem knowledge
- 🚫 yearly deprecation cycle (Kubernetes-specific)
- 🚫 early mistakes haunt us still (Persistent Volumes)
- 🚫 thread starvation is suddenly a problem
- 🚫 new types of outages
  - KEDA
  - Prometheus
  - certmanager
  - Kubernetes APIServer
  - linkerd
  - “why are there 200 Pending pods” (Azure VM allocation failures (invisible))
  - why has calico restarted 300 times

Ongoing Tweaks

- Container CPU/RAM right-sizing
- Autoscaling - CPU to message-based scaling to complex PromQL-based scaling

Future (unrealized/hypothetical) benefits of living the container life:

- large ecosystem with a bright future
- leverage savings of ARM over x64 (roughly ~20% savings right now?)
- ratcheting up the security many ways - read-only filesystem, limited user permissions, ephemeral temp storage, CPU and RAM limits (preventing noisy neighbor problems), clamp down the network

Security vulnerability scanners:
- Reduce attack surface - dotnet/runtime-deps instead of ubuntu
- Microsoft-maintained base images are always improving
- ChainGuard base images
- 🚫 Sweep it under the rug: compile self-contained binary

### Why Containers

- In the exact same way shipping containers may be carried by 18-wheelers, trains, and huge container ships, OCI containers give us a standard way to host applications
- So containers are a portable, standard way to deploy your applications
- Portable: runs the same way\* locally, in a docker-compose setup, in the many cloud services, k8s, and whatever the future brings
- Standard: huge ecosystem, which moves the state-of-the-art faster than any single-vendor (Microsoft) solution ever could
  - e.g. community has healed around Docker Desktop moving to a paid model
  - e.g. community has provided simpler alternatives to Kubernetes

#### Choosing an Azure service

In 2023, mostly for ecosystem benefits, for the typical enterprise .NET app, choose either:

- Azure Container Apps - ok to be honest I haven't tried this
- Maybe Azure Container **Instances**? - inflexible but pure
- Azure App Services - IIS on Windows! Still good!
  - disclaimer: I am talking about "App Services on Windows" - I have no idea if anyone is using "App Services on Linux" or "multi-container app on App Services"
- Azure Kubernetes Service - "budget one full time resource just to manage"
- 🚫 I have a vendetta against Azure Functions

#### Docker Desktop, Rancher Desktop, Podman Desktop

I put myself through a few hours of punishment evaluating all of them some months ago, and here are my findings:

- Docker Desktop
  - sluggish Electron app
  - ~not free
  - will match most tutorials expecting `docker` and `docker-compose`
  - feature-rich UI, very convenient
  - best WSL support (specifically: you can run `docker` from both the Windows host and from WSL instances)
  - Kubernetes: no immediate path to easy ingresses? otherwise, identical
- Rancher Desktop
  - Admin-only app - can change settings and configuration only, not inspect or manipulate containers
  - best WSL support (specifically: you can run `nerdctl` and `kubectl` from both the Windows host and from WSL instances)
  - Kubernetes: better default ingress? otherwise, identical
- Podman Desktop
  - responsive, zippy app!
  - almost nothing in it
  - ok WSL support (specifically: you can run `podman` from the Windows host only)
  - "Deploy Kubernetes YAML" is by its very nature a flawed concept
  - `podman-compose` does not exist out of the box
  - does not auto-upgrade
  - Kubernetes: ships with hipster ingress? otherwise, identical
- honorable mention: lazydocker TUI - free, can manipulate and inspect containers - use in conjunction with Rancher Desktop, which is missing all these features in its UI?
- dishonorable mention: learning the awful `docker` CLI syntax, especially the awful positional stuff

### Takeaways for you

- **Migrating from .NET Framework to .NET Core is hard**, and after migrating, containerizing is trivial by comparison.
- **in 2023, Azure Container Apps** seem to be the best way to host containerized apps, and Azure App Services is still perfect for Windows/IIS hosting
- **Default to stateless services running in containers** going forward - you will be in the best position to move anywhere in the future
- **Avoid Kubernetes** unless you need it - maintenance alone will require roughly 1 FTE to maintain
- **Docker Desktop** may cost you money, but it's the best option for local development. **Rancher Desktop** is also good, and you can also be successful with **Podman Desktop**.
- Threading might be a new problem, because containers run in cgroups and linux's Completely Fair Scheduler

### Kubernetes-specific takeaways

- **use `k9s` or OpenLens** to help navigate your many Kubernetes Resources - much more discoverable than `kubectl`
- For ecosystem benefits, **deploy your code via Helm charts** - kustomize is inferior but ok
- For Helm charts, **practice YAGNI** - make variables only for things that vary - do not ever imitate bitnami Helm charts, not ever
- 🚫 **GitOps deployment workflow is too rigid**, but you are welcome to try - probably good for "mature teams"
- for most workloads, you can get by with mastering just a tiny part of Kubernetes: **Container -> Pod -> Deployment -> Service -> Ingress** - and nothing else!
