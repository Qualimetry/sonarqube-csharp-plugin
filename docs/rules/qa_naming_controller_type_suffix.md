# Controller types should carry a 'Controller' suffix

`qa_naming_controller_type_suffix` &middot; Naming &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Web frameworks discover and route to controllers by convention, and tooling, logs, and routes all read better when a controller is named for what it is. A type that derives from a controller base but omits the `Controller` word hides its role and can fall outside convention-based discovery.

## Noncompliant code example

```csharp
public class ControllerBase
{
}

public class Account : ControllerBase // Noncompliant
{
}
```

## Compliant solution

```csharp
public class ControllerBase
{
}

public class AccountController : ControllerBase
{
}
```

## See also

- [ASP.NET Core controllers](https://learn.microsoft.com/aspnet/core/mvc/controllers/actions)
