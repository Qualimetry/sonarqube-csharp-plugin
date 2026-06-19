# Classes should not sit too deep in the inheritance hierarchy

`qa_metrics_deep_inheritance` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

Each level of inheritance adds context a reader must carry to understand a class: inherited state, virtual members that may be overridden, and constructor chaining. A deep chain makes behaviour hard to predict from any single file. Measuring the distance from a class to the root of its base chain flags hierarchies that have grown unwieldy and might be better expressed through composition.

The depth limit is configurable.

## Noncompliant code example

```csharp
public class A { }
public class B : A { }
public class C : B { }
public class D : C { }
public class E : D { } // Noncompliant
```

## Compliant solution

```csharp
public class Animal { }
public class Dog : Animal { }
```

## Parameters

This rule is configurable. Edit `maxdepth` (default `5`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
