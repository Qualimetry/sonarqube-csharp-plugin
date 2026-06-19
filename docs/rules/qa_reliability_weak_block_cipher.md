# Weak block ciphers such as DES and Triple DES should not be used

`qa_reliability_weak_block_cipher` &middot; Reliability &middot; Vulnerability &middot; severity CRITICAL &middot; enabled in the recommended profile

DES has a 56-bit key that is trivially brute-forced on modern hardware, and Triple DES inherits a small 64-bit block that makes it vulnerable to birthday attacks on large volumes of data. Constructing either algorithm, or asking a factory for one by name, ties new code to cryptography that is no longer considered safe.

Use a current authenticated cipher such as AES instead, and let the platform pick a secure implementation through its factory method.

## Noncompliant code example

```csharp
using System.Security.Cryptography;

public sealed class Cipher
{
    public SymmetricAlgorithm Create() => TripleDES.Create(); // Noncompliant
}
```

## Compliant solution

```csharp
using System.Security.Cryptography;

public sealed class Cipher
{
    public SymmetricAlgorithm Create() => Aes.Create();
}
```

## See also

- [CWE-327: Use of a Broken or Risky Cryptographic Algorithm](https://cwe.mitre.org/data/definitions/327.html)
