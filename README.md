<h1 align="center">🛒 E-Commerce Microservices Architecture</h1>

<p align="center">
  Architecture microservices complète .NET 7 — API Gateway, Service Discovery, Messaging asynchrone, Monitoring<br>
  <b>Ocelot · Consul · Kafka · Prometheus · Grafana · Docker · Kubernetes</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET_7-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET">
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker">
  <img src="https://img.shields.io/badge/Kubernetes-326CE5?style=for-the-badge&logo=kubernetes&logoColor=white" alt="Kubernetes">
  <img src="https://img.shields.io/badge/Apache_Kafka-231F20?style=for-the-badge&logo=apachekafka&logoColor=white" alt="Kafka">
  <img src="https://img.shields.io/badge/Consul-F24C53?style=for-the-badge&logo=consul&logoColor=white" alt="Consul">
  <img src="https://img.shields.io/badge/Prometheus-E6522C?style=for-the-badge&logo=prometheus&logoColor=white" alt="Prometheus">
  <img src="https://img.shields.io/badge/Grafana-F46800?style=for-the-badge&logo=grafana&logoColor=white" alt="Grafana">
  <img src="https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white" alt="MongoDB">
  <img src="https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white" alt="MySQL">
  <img src="https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white" alt="Angular">
</p>

---

## 📌 Présentation

Architecture **microservices** complète comprenant **3 services métier indépendants** (Product, Order, Client), exposés via un **API Gateway (Ocelot)**, enregistrés dans un **Service Registry (Consul)**, communiquant de façon **asynchrone via Kafka**, avec **monitoring temps réel (Prometheus + Grafana)**, et orchestrés avec **Docker** et **Kubernetes**.

Chaque service possède sa **propre base de données** (polyglot persistence) — MongoDB, MySQL, SQL Server.

---

## 🏗️ Architecture

```text
                       ┌──────────────────────┐
                       │   Service Registry   │
                       │      (Consul)        │
                       │   localhost:8500     │
                       └──────────┬───────────┘
                                  │
                       ┌──────────▼───────────┐
                       │     API Gateway      │
                       │      (Ocelot)        │
                       │   localhost:5000     │
                       └──────────┬───────────┘
                                  │
      ┌───────────────────────────┼───────────────────────────┐
      │                           │                           │
┌─────▼────────┐         ┌────────▼────────┐         ┌────────▼────────┐
│   Product    │         │      Order      │         │     Client      │
│ Microservice │◄───────►│   Microservice  │◄───────►│  Microservice   │
│  .NET 7      │         │    .NET 7       │         │    .NET 7       │
│  :5001       │         │    :5002        │         │    :5003        │
└──────┬───────┘         └────────┬────────┘         └────────┬────────┘
       │                          │                           │
  ┌────▼─────┐            ┌───────▼──────┐            ┌───────▼──────┐
  │ MongoDB  │            │    MySQL     │            │  SQL Server  │
  │  7.0     │            │    8.0       │            │              │
  └──────────┘            └──────────────┘            └──────────────┘
                                  ▲
                                  │
                       ┌──────────┴──────────┐
                       │   Apache Kafka      │
                       │   + Zookeeper       │
                       │  (async messaging)  │
                       └─────────────────────┘

                       ┌─────────────────────┐
                       │    Prometheus       │  ◄── scrape /metrics
                       │   localhost:9090    │
                       └──────────┬──────────┘
                                  │
                       ┌──────────▼──────────┐
                       │      Grafana        │  ◄── dashboards
                       │   localhost:3000    │
                       └─────────────────────┘
```

---

## 🧩 Composants

| Composant | Rôle | Technologie | Port |
|-----------|------|-------------|------|
| **API Gateway** | Point d'entrée unique, routage | Ocelot | `5000` |
| **Service Registry** | Découverte dynamique de services | Consul | `8500` |
| **Product Service** | Gestion du catalogue produits | .NET 7 + MongoDB | `5001` |
| **Order Service** | Gestion des commandes | .NET 7 + MySQL | `5002` |
| **Client Service** | Gestion des clients | .NET 7 + SQL Server | `5003` |
| **Apache Kafka** | Bus de messages asynchrone | Kafka + Zookeeper | `9092` |
| **Prometheus** | Collecte & stockage de métriques | Prometheus | `9090` |
| **Grafana** | Visualisation & dashboards | Grafana | `3000` |
| **Frontend** | Interface utilisateur | Angular | `4200` |
| **Orchestration** | Déploiement & scaling | Docker + Kubernetes | — |

---

## 🧰 Stack technique

### Backend
- **.NET 7** (ASP.NET Core Web API)
- **Ocelot** — API Gateway
- **Consul** — Service Discovery
- **Confluent.Kafka** — Messaging asynchrone
- **Entity Framework Core** — MySQL / SQL Server
- **MongoDB.Driver** — Driver MongoDB

### Bases de données (Polyglot Persistence)
- 🍃 **MongoDB 7.0** → `ProductDb` (ProductService)
- 🐬 **MySQL 8.0** → `OrderDb` (OrderService)
- 🟥 **SQL Server** → `ClientDb` (ClientService)

### Monitoring & Observabilité
- 📊 **Prometheus** — Collecte des métriques (scrape `/metrics`)
- 📈 **Grafana** — Visualisation & dashboards temps réel

### Frontend
- 🅰️ **Angular** (dossier `frontend/`)

### Infrastructure
- 🐳 **Docker** & **Docker Compose**
- ☸️ **Kubernetes** (Minikube)

---

## 📁 Structure du projet

```text
MicroservicesTP/
├── ApiGateway/              # Passerelle Ocelot (port 5000)
│   ├── Program.cs
│   ├── ocelot.json
│   ├── appsettings.json
│   └── Dockerfile
│
├── ProductService/          # Catalogue produits (MongoDB)
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Settings/
│   └── Dockerfile
│
├── OrderService/            # Commandes (MySQL)
│   ├── Controllers/
│   ├── Data/
│   ├── Migrations/
│   ├── Models/
│   └── Dockerfile
│
├── ClientService/           # Clients (SQL Server)
│   ├── Controllers/
│   ├── Models/
│   └── Dockerfile
│
├── frontend/                # Application Angular
│   └── src/
│
├── docker-compose.yml       # Orchestration complète
├── prometheus.yml           # Config scraping Prometheus
├── MicroservicesTP.slnx
└── README.md
```

---

## 🚀 Lancement rapide

### Prérequis

| Outil | Lien |
|-------|------|
| .NET 7 SDK | https://dotnet.microsoft.com/download |
| Docker Desktop | https://www.docker.com/products/docker-desktop |
| Node.js ≥ 18 | https://nodejs.org |
| kubectl + minikube | https://kubernetes.io/docs/tasks/tools/ |

Vérification :

```bash
docker --version
dotnet --version
node --version
```

---

### 🐳 Démarrage en une commande

```bash
docker-compose up -d --build
```

Docker Compose va démarrer **11 conteneurs** :

- ✅ `consul` — Service Registry
- ✅ `mongodb` — Base produits
- ✅ `mysql` — Base commandes
- ✅ `zookeeper` + `kafka` — Messaging
- ✅ `prometheus` + `grafana` — Monitoring
- ✅ `product-service` + `order-service` + `client-service` — Microservices
- ✅ `api-gateway` — Point d'entrée

---

### 🌐 Accès aux services

| Service | URL | Description |
|---------|-----|-------------|
| 🚪 **API Gateway** | http://localhost:5000 | Point d'entrée unique |
| 🛍️ **Product Service** | http://localhost:5001 | API produits |
| 📦 **Order Service** | http://localhost:5002 | API commandes |
| 👤 **Client Service** | http://localhost:5003 | API clients |
| 🔍 **Consul UI** | http://localhost:8500/ui | Service discovery |
| 📊 **Prometheus UI** | http://localhost:9090 | Métriques |
| 📈 **Grafana** | http://localhost:3000 | Dashboards |
| 🅰️ **Frontend Angular** | http://localhost:4200 | UI |

> 🔐 **Grafana** : login `admin` / mot de passe `admin`

---

## 📊 Monitoring avec Prometheus & Grafana

### Configuration Prometheus (`prometheus.yml`)

```yaml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'api-gateway'
    metrics_path: /metrics
    static_configs:
      - targets: ['api-gateway:8080']

  - job_name: 'product-service'
    static_configs:
      - targets: ['product-service:8080']

  - job_name: 'order-service'
    static_configs:
      - targets: ['order-service:8080']

  - job_name: 'client-service'
    static_configs:
      - targets: ['client-service:8080']
```

### Visualisation dans Grafana

1. Ouvre http://localhost:3000 (admin / admin)
2. Ajoute une **datasource Prometheus** → URL : `http://prometheus:9090`
3. Importe un dashboard (ID **`19924`** pour ASP.NET Core, ou **`10427`** pour .NET générique)
4. Visualise : requêtes/sec, latence, erreurs HTTP, mémoire, CPU…

---

## 🔄 Communication asynchrone (Kafka)

### Producteur — `OrderService`

Publie un événement à chaque commande créée :

```csharp
var producer = new ProducerBuilder<Null, string>(
    new ProducerConfig { BootstrapServers = "kafka:29092" }).Build();

await producer.ProduceAsync("order-created",
    new Message<Null, string> { Value = JsonSerializer.Serialize(order) });
```

### Consommateur — `ClientService`

Réagit à l'événement en temps réel :

```csharp
var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig {
    BootstrapServers = "kafka:29092",
    GroupId = "client-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
}).Build();

consumer.Subscribe("order-created");
```

> 💡 Ce pattern **découple** les services : `OrderService` n'a pas besoin de connaître `ClientService`.

---

## 🔌 Endpoints principaux

### Via API Gateway (`http://localhost:5000`)

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `GET` | `/products` | Lister les produits |
| `POST` | `/products` | Créer un produit |
| `PUT` | `/products/{id}` | Modifier un produit |
| `DELETE` | `/products/{id}` | Supprimer un produit |
| `GET` | `/orders` | Lister les commandes |
| `POST` | `/orders` | Créer une commande |
| `GET` | `/clients` | Lister les clients |
| `POST` | `/clients` | Créer un client |

---

## 🧪 Tests & Validation

| # | Action | Endpoint | Résultat attendu |
|---|--------|----------|------------------|
| 1 | Créer un produit | `POST /products` | `201 Created` + ID généré |
| 2 | Lister les produits | `GET /products` | `200 OK` — tableau JSON |
| 3 | Créer une commande | `POST /orders` | `201 Created` + événement Kafka publié |
| 4 | Créer un client | `POST /clients` | `201 Created` |
| 5 | Vérifier Consul | `http://localhost:8500/ui` | Les 3 services `PASSING` |
| 6 | Vérifier Prometheus | `http://localhost:9090/targets` | Tous les targets `UP` |
| 7 | Vérifier Grafana | `http://localhost:3000` | Dashboards actifs |

### Exemples cURL

```bash
# Créer un produit
curl -X POST http://localhost:5000/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Laptop","price":99.99,"stock":50}'

# Créer une commande
curl -X POST http://localhost:5000/orders \
  -H "Content-Type: application/json" \
  -d '{"productId":"abc123","clientId":1,"quantity":2}'

# Créer un client
curl -X POST http://localhost:5000/clients \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Alice Dupont","email":"alice@example.com"}'
```

### Vérifier les conteneurs

```bash
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

---

## ☸️ Déploiement Kubernetes

```bash
# Démarrer Minikube
minikube start

# Appliquer les manifests
kubectl apply -f k8s/

# Vérifier
kubectl get pods
kubectl get services
```

Exemple de `deployment.yaml` :

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: product-service
spec:
  replicas: 2
  selector:
    matchLabels:
      app: product-service
  template:
    metadata:
      labels:
        app: product-service
    spec:
      containers:
        - name: product-service
          image: product-service:v1
          ports:
            - containerPort: 8080
```

---

## ⚙️ Configuration

### API Gateway (`ocelot.json`)

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/products/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [{ "Host": "product-service", "Port": 8080 }],
      "UpstreamPathTemplate": "/products/{everything}",
      "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"]
    }
  ],
  "GlobalConfiguration": { "BaseUrl": "http://localhost:5000" }
}
```

### Variables d'environnement (Docker Compose)

Chaque microservice est configuré via variables d'environnement :

- `Consul__Address` → `http://consul:8500`
- `Kafka__BootstrapServers` → `kafka:29092`
- `MongoDbSettings__ConnectionString` → `mongodb://mongodb:27017`
- `ConnectionStrings__DefaultConnection` → connexion MySQL

---

## 🛣️ Améliorations possibles

- [ ] 🔐 Ajouter authentification JWT (IdentityServer / Keycloak)
- [ ] 📨 Ajouter un service **Notification** (email/SMS) via Kafka
- [ ] 🧪 Tests unitaires & d'intégration (xUnit, Testcontainers)
- [ ] 🔄 CI/CD avec **GitHub Actions**
- [ ] 📦 Publication sur **Docker Hub**
- [ ] 🌐 Ajout **Jaeger / OpenTelemetry** (tracing distribué)
- [ ] 🚦 Ajout **Rate Limiting** sur l'API Gateway
- [ ] 🔁 Ajout **Circuit Breaker** (Polly)

---

## 📚 Ressources

- [Documentation .NET](https://learn.microsoft.com/dotnet/)
- [Ocelot API Gateway](https://ocelot.readthedocs.io/)
- [Consul](https://developer.hashicorp.com/consul/docs)
- [Apache Kafka](https://kafka.apache.org/documentation/)
- [Prometheus](https://prometheus.io/docs/)
- [Grafana](https://grafana.com/docs/)
- [Kubernetes](https://kubernetes.io/docs/)

---

## 📄 Licence

Projet pédagogique — **Travaux Pratiques**. Libre d'utilisation à des fins éducatives.

---

## 👩‍💻 Auteure

**Malak Khatir**

<p align="center">
  ⭐ Si ce projet t'aide, n'oublie pas de lui donner une étoile ! ⭐
</p>
