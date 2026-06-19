# Commented-out code should be removed

`qa_style_commented_out_code` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

A comment that holds a disabled statement is dead weight: it never runs, the compiler never checks it, and it drifts out of step with the surrounding code until nobody trusts it. Version control already keeps the old text, so the commented statement adds risk without adding value.

This rule flags single-line comments whose content reads as a discarded statement, recognised by a trailing statement terminator or block brace. Keep comments that explain intent and delete the ones that merely park old code.

## Noncompliant code example

```csharp
public int Total(int price, int quantity)
{
    // total = price * quantity * taxRate; // Noncompliant
    return price * quantity;
}
```

## Compliant solution

```csharp
public int Total(int price, int quantity)
{
    // Tax is applied later by the billing service.
    return price * quantity;
}
```
