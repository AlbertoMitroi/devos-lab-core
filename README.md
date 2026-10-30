# lab-core

Shared NuGet packages of the equipment-leasing lab, published to GitHub Packages (`https://nuget.pkg.github.com/AlbertoMitroi/index.json`).

| Package | Content |
|---|---|
| `Lab.Core.Transform.Bpmn` | `BpmnProcessLoader`: reads `.bpmn` files and their `*.ProcessMetaData.json` into a `ProcessDefinition` |
| `Lab.Core.Workflow.EventFlow` | `FlowAggregate`, flow events, `EventFlowAnalyzer` (open tasks, available actions, status of a flow), plugin interfaces `IHandleServiceTask`, `IHandleUserTaskCommand<T>`, `IHandleAction`, `IHandleNewFlow` and their naming conventions |
| `Lab.Core.MessageHubs` | `IMessageHubUtility` (`SendToAll`, `SendToSpecifiedUsers`) over a SignalR hub |
| `Lab.Core.Clients.Pricing` | `IRateCardClient`, typed client for the rate card and residual value endpoints of the Pricing API (`Clients:RateCards:BaseAddress`) |

## Build

Docker only:

```bash
docker build --target test .
docker build --target packages --build-arg VERSION=1.0.0 --output type=local,dest=out .
```

A tag `v*` publishes the packages from GitHub Actions.

## License

MIT, see [LICENSE](LICENSE).
