# Certificate validation must not be disabled

`qa_reliability_certificate_validation_disabled` &middot; Reliability &middot; Vulnerability &middot; severity BLOCKER &middot; enabled in the recommended profile

Assigning a certificate-validation callback that always returns `true` turns off transport security: the connection will trust any certificate, including a forged or expired one, which opens the door to man-in-the-middle interception. The callback must inspect the certificate, the chain, and the policy errors and reject anything it cannot verify.

## Noncompliant code example

```csharp
client.ServerCertificateCustomValidationCallback =
    (request, cert, chain, errors) => true; // Noncompliant
```

## Compliant solution

```csharp
client.ServerCertificateCustomValidationCallback =
    (request, cert, chain, errors) => errors == SslPolicyErrors.None;
```

## See also

- [CWE-295: Improper Certificate Validation](https://cwe.mitre.org/data/definitions/295.html)
