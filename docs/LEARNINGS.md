# What I learned building this Payment Service

This document is a personal learning log from building a **basic Payment API** with ASP.NET Core 8.

## 1. Domain modeling comes first

Before writing endpoints, I modeled the payment world in English types:

| Model | Role |
|-------|------|
| `CreatePaymentRequest` | What the client sends |
| `CardInfo` / `BuyerInfo` / `AddressInfo` / `BasketItem` | Nested payment context |
| `Payment` | Stored payment record |
| `PaymentResult` | API response shape |
| `PaymentStatus` | Lifecycle: Pending, Succeeded, Failed, ... |

**Takeaway:** Clear models make the rest of the API obvious.

## 2. Layering without over-engineering

```
Controllers  → HTTP / status codes
Services     → business rules + mock provider
Store        → in-memory persistence (learning stand-in for DB)
Models       → shared contracts
Program.cs   → DI + pipeline only
```

I intentionally avoided microservices, gateways, and real PSP SDKs.
The goal was understanding the **shape** of a payment service, not shipping production payments.

## 3. A payment service is mostly rules

Even a mock provider needs:

- Required field validation
- Basket total must match amount
- Card format checks (length + Luhn)
- Demo success / decline cards
- Duplicate `orderNumber` protection
- Status mapping into a consistent `PaymentResult`

**Takeaway:** “Charge the card” is one step; **validation + idempotency + status** are the real service.

## 4. Interfaces make replacement obvious

```csharp
IPaymentService  → can later wrap Iyzico / Stripe
IPaymentStore    → can later become EF Core / Redis
```

Today: mock + memory.  
Tomorrow: swap implementations without rewriting controllers.

## 5. Demo cards teach provider behavior

| Card | Behavior |
|------|----------|
| `5528790000000008` | Success |
| `5406670000000009` | Decline |
| Other Luhn-valid | Success under amount limit |

This mimics how sandboxes work: controlled outcomes for learning and tests.

## 6. What I deliberately did NOT do

- Real PCI card handling in production
- 3-D Secure browser flow end-to-end
- Database / outbox / messaging
- AuthN/AuthZ
- Multi-tenant merchant accounts

Those are next steps after the basics click.

## 7. How to run the learning loop

1. Start API → open Swagger
2. POST a success card payment
3. GET by `paymentId`
4. POST a decline card → read `Failed` status
5. Break basket total → see validation message

Understanding comes from **changing one input and watching the result**, not from reading SDK docs alone.
