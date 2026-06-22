# Types with poor cohesion should be split

`qa_metrics_type_poor_cohesion` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

When most methods in a type touch disjoint sets of fields, the type is doing unrelated work. Split it into focused types with clearer responsibilities.

## Noncompliant code example

```csharp
public sealed class KitchenSink
{
    private int _invoiceCount;
    private int _shipmentCount;
    private int _refundCount;
    private int _inventoryCount;

    public void CountInvoice() => _invoiceCount++;
    public void CountShipment() => _shipmentCount++;
    public void CountRefund() => _refundCount++;
    public void CountInventory() => _inventoryCount++;
}
```

## Compliant solution

```csharp
public sealed class InvoiceTracker
{
    private int _invoiceCount;
    public void CountInvoice() => _invoiceCount++;
}

public sealed class ShipmentTracker
{
    private int _shipmentCount;
    public void CountShipment() => _shipmentCount++;
}
```
