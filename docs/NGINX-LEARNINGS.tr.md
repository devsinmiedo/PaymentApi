# Nginx Load Balancer — Öğrendiklerim

Payment API’nin yanında, aynı backend’lerin önünde trafiğin **nasıl dağıtıldığını** nginx load balancer ile deneyimledim.

Lab projesi (kardeş repo): `nginx-load-balancer-lab`

## Kullandığım kurulum

```text
İstemci / load generator
        │
        ▼
   Nginx (:8080)   ← STRATEGY= ile algoritma değişir
        │
   backend-1 .. backend-4   (her biri instanceId döner)
```

Aynı backend’ler, farklı `upstream` algoritmaları — deneyin tamamı buydu.

## Çalıştırdığım stratejiler

| Strateji | nginx fikri | Gözlemlediğim |
|----------|-------------|----------------|
| **round-robin** | default upstream | `worker_processes 1` ile 100 istekte **25/25/25/25** |
| **weighted** | `backend-1 weight=3` | backend-1 yaklaşık 3 kat fazla trafik |
| **least-conn** | `least_conn` + yavaş backend-3 | backend-3 **çok daha az** istek (örn. 5’e karşı 25) |
| **ip-hash** | `ip_hash` | Tek istemciden gelen tüm istekler **tek** backend’e (sticky) |
| **hash** | `hash $request_uri consistent` | Aynı URI aynı peer’e yönelir |

## Ana kazanımlar

1. **Load balancing sihir değil** — `upstream { }` içinde seçilen algoritmadır.
2. **Round-robin demoda dikkat** — her nginx worker kendi sayacını tutar; tek worker lab sonucunu okunaklı yapar.
3. **least_conn asimetri ister** — herkes aynı hızdaysa algoritmayı göremezsin; yavaş instance farkı netleştirir.
4. **ip-hash = istemci IP’ye göre yapışkanlık** — tam session store değildir; sticky routing zihinsel modeli.
5. **C4 altyapıyı anlatmaya yardım eder** — Context → Container → Component (nginx içi), sonra conf dosyaları.

## Kanıtlayan komutlar

```bash
cd ~/Projects/nginx-load-balancer-lab
STRATEGY=round-robin docker compose up -d --build

./scripts/switch-strategy.sh round-robin
./clients/load-generator/run.sh 100 10 /

./scripts/switch-strategy.sh least-conn
./clients/load-generator/run.sh 80 20 /work

./scripts/switch-strategy.sh ip-hash
./clients/load-generator/run.sh 50 5 /
```

## PaymentApi ile bağlantısı

PaymentApi → ödeme servisinin **içi**.  
Nginx lab → birçok instance’ın **önündeki** trafik dağıtımı.

Birlikte: uygulama tasarımı + yük dengeleme.
