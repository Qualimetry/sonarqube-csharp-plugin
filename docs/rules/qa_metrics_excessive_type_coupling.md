# Types should not reference too many other types

`qa_metrics_excessive_type_coupling` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

A type whose members mention a large number of other types tends to sit at the centre of many responsibilities and is hard to change without ripple effects. Counting the distinct types named across a type's fields, properties, and method signatures gives a quick read on how tightly it is wired to the rest of the code base.

The limit is configurable so that gateway or composition types, where a wide surface is expected, can be tuned separately.

## Noncompliant code example

```csharp
public sealed class OrderProcessor
{
    private Catalog _catalog;
    private Inventory _inventory;
    private Pricing _pricing;
    private Shipping _shipping;
    private Billing _billing;
    private Auditor _auditor;

    public Receipt Handle(Cart cart, Customer customer, Coupon coupon)
    {
        return new Receipt();
    }
}
```

## Compliant solution

```csharp
public sealed class OrderProcessor
{
    private readonly IOrderPipeline _pipeline;

    public OrderProcessor(IOrderPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public Receipt Handle(Cart cart)
    {
        return _pipeline.Run(cart);
    }
}
```

## Parameters

This rule is configurable. Edit `maxcoupling` (default `20`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
