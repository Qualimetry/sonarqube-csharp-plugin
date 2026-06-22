# Namespace dependency cycles should be avoided

`qa_architecture_namespace_dependency_cycle` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Cyclic dependencies between namespaces make layering hard to reason about and block clean refactors. Break the cycle by introducing a shared abstraction or moving shared types to a neutral namespace.

## Noncompliant code example

```csharp
namespace Alpha.Services
{
    public sealed class AlphaService
    {
        public void Run(BetaServices.Client client) => client.Execute();
    }
}

namespace BetaServices
{
    public sealed class Client
    {
        public void Execute() => new Alpha.Services.AlphaService().Run(this);
    }
}
```

## Compliant solution

```csharp
namespace Alpha.Contracts
{
    public interface IClient
    {
        void Execute();
    }
}

namespace Alpha.Services
{
    public sealed class AlphaService
    {
        public void Run(IClient client) => client.Execute();
    }
}

namespace Beta.Client
{
    public sealed class Client : IClient
    {
        public void Execute() { }
    }
}
```
