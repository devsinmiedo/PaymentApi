# Architecture (learning view)

Simple layered ASP.NET Core 8 API — educational, not production.

```mermaid
C4Container
title Basic Payment Service - Containers (learning)

Person(dev, "Developer", "Calls Swagger / HTTP client")

Container_Boundary(api, "PaymentApi") {
    Container(controller, "PaymentsController", "ASP.NET Core", "POST/GET /api/payments")
    Container(service, "PaymentService", "C#", "Validation + mock provider rules")
    Container(store, "InMemoryPaymentStore", "ConcurrentDictionary", "Temporary payment records")
}

Rel(dev, controller, "HTTP JSON")
Rel(controller, service, "Create / Get / List")
Rel(service, store, "Save / Read")
```

## Request flow

```text
Client
  → PaymentsController.Create(CreatePaymentRequest)
    → PaymentService.CreatePayment()
         1) Validate request
         2) Reject duplicate orderNumber
         3) Evaluate demo card rules
         4) Save Payment
         5) Return PaymentResult
  ← 200 OK / 400 Bad Request
```

## Why in-memory store?

For learning:

- Zero infrastructure
- Instant feedback
- Same interface later maps to SQL

Data resets when the process stops — that is intentional.
