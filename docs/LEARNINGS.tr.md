# Bu Payment Service’i yazarken öğrendiklerim

Bu not, **ASP.NET Core 8** ile temel bir Payment API kurarken tuttuğum öğrenme günlüğü.

## 1. Önce domain modeli

Endpoint yazmadan önce ödeme dünyasını modellerle tanımladım:

| Model | Rol |
|-------|-----|
| `CreatePaymentRequest` | İstemcinin gönderdiği istek |
| `CardInfo` / `BuyerInfo` / `AddressInfo` / `BasketItem` | Ödeme bağlamı |
| `Payment` | Saklanan kayıt |
| `PaymentResult` | API cevabı |
| `PaymentStatus` | Durum: Pending, Succeeded, Failed, ... |

**Kazanım:** Model net olunca API kendiliğinden şekilleniyor.

## 2. Aşırıya kaçmadan katmanlar

```text
Controllers  → HTTP / status code
Services     → iş kuralları + mock provider
Store        → bellek (öğrenme için DB yerine)
Models       → ortak sözleşmeler
Program.cs   → sadece DI + pipeline
```

Bilinçli olarak mikroservis, gateway ve gerçek PSP SDK’sı yok.
Amaç production ödemesi değil; **servisin iskeletini** kavramak.

## 3. Ödeme servisi çoğunlukla kural demektir

Mock bile olsa:

- Zorunlu alan doğrulama  
- Sepet toplamı = tutar  
- Kart formatı (uzunluk + Luhn)  
- Demo başarı / red kartları  
- Aynı `orderNumber` tekrarı engeli  
- Sonucun `PaymentResult`’a map’lenmesi  

**Kazanım:** “Kartı çek” tek adımdır; asıl iş **validation + durum + tekrar güvenliği**.

## 4. Interface = ileride değiştirilebilirlik

```csharp
IPaymentService  → sonra Iyzico / Stripe
IPaymentStore    → sonra EF Core / Redis
```

Bugün: mock + memory.  
Yarın: controller’lara dokunmadan implementasyon değişir.

## 5. Demo kartlar provider davranışını öğretir

| Kart | Davranış |
|------|----------|
| `5528790000000008` | Başarılı |
| `5406670000000009` | Red |
| Diğer Luhn-geçerli | Limit altındaysa başarılı |

Sandbox mantığı: kontrollü sonuçlarla öğrenme ve test.

## 6. Bilinçli olarak yapmadıklarım

- Gerçek PCI / canlı kart  
- Uçtan uca 3D Secure tarayıcı akışı  
- Veritabanı / outbox / mesajlaşma  
- Auth  
- Multi-tenant merchant  

Temel oturunca sıradaki konular bunlar.

## 7. Öğrenme döngüsü

1. API’yi çalıştır → Swagger  
2. Başarılı kartla POST  
3. `paymentId` ile GET  
4. Red kartı dene → `Failed`  
5. Sepet toplamını boz → validation mesajını gör  

Anlamak, SDK dokümanı okumaktan çok **bir girdiyi değiştirip sonucu izlemekten** geliyor.
