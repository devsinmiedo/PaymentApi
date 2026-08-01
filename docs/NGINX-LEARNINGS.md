# Nginx Load Balancer — What I learned

Alongside the Payment API, I practiced **how traffic is distributed** in front of multiple identical backends using **nginx** as a load balancer.

Lab repo (sibling project): `nginx-load-balancer-lab`

## Setup I used

```text
Client / load generator
        │
        ▼
   Nginx (:8080)   ← strategy switched via STRATEGY=
        │
   backend-1 .. backend-4   (each returns instanceId)
```

Same backends, different `upstream` algorithms — that was the whole experiment.

## Strategies I ran

| Strategy | nginx idea | What I observed |
|----------|------------|-----------------|
| **round-robin** | default upstream | With `worker_processes 1` → **25/25/25/25** on 100 requests |
| **weighted** | `weight=3` on backend-1 | backend-1 got ~3× more traffic |
| **least-conn** | `least_conn` + slow backend-3 | backend-3 got **far fewer** hits (e.g. 5 vs 25) |
| **ip-hash** | `ip_hash` | All requests from one client → **one** backend (50/50 sticky) |
| **hash** | `hash $request_uri consistent` | Same URI tends to same peer |

## Key lessons

1. **Load balancing is not magic** — it is an explicit algorithm in `upstream { }`.
2. **Round-robin needs care in demos** — multiple nginx workers each keep their own counter; one worker makes lab results readable.
3. **least_conn needs asymmetry** — if every node is equally fast, you cannot see the algorithm; a slow instance makes it obvious.
4. **ip-hash = affinity by client IP** — useful mental model for “sticky” routing (not a full session store).
5. **C4 helps explain infra** — Context → Container → Component (nginx internals) before diving into conf files.

## Commands that proved the point

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

## How this connects to PaymentApi

PaymentApi taught me the **inside** of a payment service.  
Nginx lab taught me what sits **in front** of many service instances in real systems.

Together: application design + traffic distribution.
