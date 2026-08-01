# PaymentApi — Temel Ödeme Servisi (Öğrenme Projesi)

> **ASP.NET Core 8** ile yazılmış, ödeme servisinin nasıl kurulduğunu kavramak için pratik bir API.  
> Modeller, doğrulama, mock provider kuralları ve katmanlı yapı.  
> **Production değildir. Gerçek para işlemez.**

[English README](README.md) · [Öğrenme notları](docs/LEARNINGS.tr.md) · [Nginx öğrenimi](docs/NGINX-LEARNINGS.tr.md) · [Mimari](docs/ARCHITECTURE.md)

---

## Bu repo neden var?

Temel payment backend kavramlarını öğrenirken bunu yazdım:

1. Bir ödeme isteğinde gerçekten hangi veriler gerekir?  
2. **HTTP**, **iş kuralları** ve **saklama** nasıl ayrılır?  
3. Sandbox’taki **başarılı / red kartları** provider davranışını nasıl öğretir?  
4. Interface’ler ileride Iyzico/Stripe değişimini nasıl kolaylaştırır?  

Aynı öğrenme döneminde **nginx load balancer** stratejilerini de ayrı bir lab’de deneyimledim (round-robin, weighted, least-conn, ip-hash). Ödeme servisinin *içini* ve trafiğin *önünü* birlikte kavramak için.

Bu depo, o yolculuğun pratik defteri ve portföy notu.

## Öğrenme yolculuğu (özet)

| Konu | Ne yaptım | Kanıt / not |
|------|-----------|-------------|
| **Payment service** | ASP.NET Core 8 API: model → service → controller | Bu repo |
| **Mock provider** | Başarı / red demo kartları, validation, status | `Services/PaymentService.cs` |
| **Nginx LB** | 4 backend + strateji değiştirerek dağılımı ölçtüm | [docs/NGINX-LEARNINGS.tr.md](docs/NGINX-LEARNINGS.tr.md) · kardeş lab: `nginx-load-balancer-lab` |
| **C4** | Context / Container / Component ile nginx lab mimarisini çizdim | [docs/c4/](docs/c4/) |

### Nginx’de gördüğüm sonuçlar (kısa)

- **round-robin** → eşit dağılım (`25/25/25/25`)  
- **weighted** → ağırlıklı instance baskın  
- **least-conn** → yavaş instance’a daha az istek  
- **ip-hash** → aynı IP hep aynı backend (sticky)  

Detay: [Nginx öğrenim notları](docs/NGINX-LEARNINGS.tr.md)

## Ne yapıyor?

| Method | Endpoint | Amaç |
|--------|----------|------|
| `POST` | `/api/payments` | Ödeme oluştur (mock) |
| `GET` | `/api/payments/{paymentId}` | Tek ödeme getir |
| `GET` | `/api/payments` | Bellekteki ödemeleri listele |
| `GET` | `/health` | Sağlık kontrolü |

## Proje yapısı

```text
PaymentApi/
├── Controllers/     # HTTP API
├── Models/          # İstek / domain / cevap
├── Services/        # Ödeme kuralları + bellek içi store
├── docs/            # Öğrenme ve mimari notları
├── Program.cs       # Sadece DI ve pipeline
└── PaymentApi.http  # Örnek istekler
```

## Hızlı başlangıç

Gereksinim: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
cd PaymentApi
dotnet run
```

- Swagger: http://localhost:5101/swagger  
- Health: http://localhost:5101/health  

### Başarılı ödeme örneği

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

### Demo kartlar

| Kart numarası | Sonuç |
|---------------|--------|
| `5528790000000008` | Başarılı |
| `5406670000000009` | Reddedildi |

Daha fazla deneme: [`PaymentApi.http`](PaymentApi.http) ve [`docs/LEARNINGS.tr.md`](docs/LEARNINGS.tr.md).

## Bilinçli tercihler

- **Bellek içi store** — akışa odaklan, DB kurma  
- **Mock provider** — PSP hesabı olmadan sonuçları gör  

## Sonraki adımlar

- [ ] EF Core ile kalıcı kayıt  
- [ ] `IPaymentService` arkasına gerçek sandbox PSP  
- [ ] Idempotency key  
- [ ] İstemci kimlik doğrulama  

## Lisans

MIT — [LICENSE](LICENSE).
