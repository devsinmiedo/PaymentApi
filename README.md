# PaymentApi — Basic Payment Service (Learning Project)

> A hands-on **ASP.NET Core 8** payment API built to understand how a payment service is structured — models, validation, mock provider rules, and clean layering.  
> **Not for production. No real money moves here.**

[Türkçe README](README.tr.md) · [Learning log](docs/LEARNINGS.md) · [Nginx learning](docs/NGINX-LEARNINGS.md) · [Architecture](docs/ARCHITECTURE.md)

---

## Why this repo exists

I built this while learning payment backend basics:

1. What data a payment request really needs  
2. How to separate **HTTP**, **business rules**, and **storage**  
3. How sandbox-style **success/decline cards** teach provider behavior  
4. How interfaces make a future Iyzico/Stripe swap imaginable  

In the same learning period I also ran an **nginx load balancer lab** (round-robin, weighted, least-conn, ip-hash) to understand what sits *in front of* multiple service instances.

This repository is my practice ground and portfolio note for that journey.

## Learning journey (overview)

| Topic | What I did | Where |
|-------|------------|--------|
| **Payment service** | ASP.NET Core 8 API: model → service → controller | This repo |
| **Mock provider** | Demo success/decline cards, validation, status | `Services/PaymentService.cs` |
| **Nginx LB** | 4 backends + switch strategies, measure distribution | [docs/NGINX-LEARNINGS.md](docs/NGINX-LEARNINGS.md) · sibling lab: `nginx-load-balancer-lab` |
| **C4** | Drew Context / Container / Component for the nginx lab | [docs/c4/](docs/c4/) |

### Nginx results I personally verified

- **round-robin** → even split (`25/25/25/25`)  
- **weighted** → heavy instance dominates  
- **least-conn** → slow instance gets fewer requests  
- **ip-hash** → one client IP sticks to one backend  

Details: [Nginx learning notes](docs/NGINX-LEARNINGS.md)

## What it does

| Method | Endpoint | Purpose |
|--------|----------|---------|
| `POST` | `/api/payments` | Create a payment (mock provider) |
| `GET` | `/api/payments/{paymentId}` | Fetch one payment |
| `GET` | `/api/payments` | List in-memory payments |
| `GET` | `/health` | Liveness check |

## Project structure

```text
PaymentApi/
├── Controllers/     # HTTP API
├── Models/          # Request / domain / response
├── Services/        # Payment rules + in-memory store
├── docs/            # Learnings & architecture notes
├── Program.cs       # DI & pipeline only
└── PaymentApi.http  # Sample requests
```

## Quick start

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
cd PaymentApi
dotnet run
```

- Swagger: http://localhost:5101/swagger  
- Health: http://localhost:5101/health  

### Success example

```bash
curl -s http://localhost:5101/api/payments -H "Content-Type: application/json" -d '{
  "orderNumber": "ORD-1001",
  "amount": 150.50,
  "currency": "TRY",
  "card": {
    "cardHolderName": "John Doe",
    "cardNumber": "5528790000000008",
    "expireMonth": "12",
    "expireYear": "2030",
    "cvv": "123"
  },
  "buyer": {
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "phone": "+905551112233"
  },
  "billingAddress": {
    "contactName": "John Doe",
    "city": "Istanbul",
    "country": "Turkey",
    "address": "Demo Street 1"
  },
  "items": [
    { "id": "ITEM-1", "name": "Demo Product", "category": "General", "price": 150.50, "quantity": 1 }
  ]
}'
```

### Demo cards

| Card number | Result |
|-------------|--------|
| `5528790000000008` | Succeeded |
| `5406670000000009` | Failed (declined) |

More experiments: see [`PaymentApi.http`](PaymentApi.http) and [`docs/LEARNINGS.md`](docs/LEARNINGS.md).

## Design choices (intentional)

- **In-memory store** — focus on flow, not database setup  
- **Mock provider** — learn outcomes without PSP credentials  


## Roadmap (after the basics)

- [ ] Persist payments (EF Core)  
- [ ] Real sandbox PSP (e.g. Iyzico) behind `IPaymentService`  
- [ ] Idempotency keys  
- [ ] Auth for calling clients  

## License

MIT — see [LICENSE](LICENSE).
