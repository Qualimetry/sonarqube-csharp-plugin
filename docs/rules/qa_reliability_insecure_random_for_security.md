# System.Random should not produce security-sensitive values

`qa_reliability_insecure_random_for_security` &middot; Reliability &middot; Vulnerability &middot; severity CRITICAL &middot; enabled in the recommended profile

`System.Random` is a fast, statistically uniform generator, but its output is predictable: given a few observed values an attacker can recover the seed and reproduce the entire sequence. Using it to build a token, key, salt, nonce, or password therefore makes those secrets guessable.

When the value feeds a security decision, generate it with a cryptographically secure source such as `RandomNumberGenerator`.

## Noncompliant code example

```csharp
using System;

public sealed class Tokens
{
    public int Next()
    {
        var tokenSeed = new Random(); // Noncompliant
        return tokenSeed.Next();
    }
}
```

## Compliant solution

```csharp
using System.Security.Cryptography;

public sealed class Tokens
{
    public int Next() => RandomNumberGenerator.GetInt32(int.MaxValue);
}
```

## See also

- [CWE-338: Use of Cryptographically Weak Pseudo-Random Number Generator (PRNG)](https://cwe.mitre.org/data/definitions/338.html)
