# 📦 Order Tracking System

Sistema completo de rastreamento de pedidos desenvolvido com **.NET 10**, **RabbitMQ**, **SQL Server**, **React + Vite** e **Clean Architecture**, utilizando mensageria para processamento assíncrono.

![CI/CD Pipeline](https://github.com/wendryandrade/order-tracking/actions/workflows/ci-cd.yml/badge.svg)
![CI - Build & Test](https://github.com/wendryandrade/order-tracking/actions/workflows/ci.yml/badge.svg)

---

## 🚀 Início Rápido (Um Comando)

```powershell
docker-compose up -d --build
```

**Aguarde ~2 minutos e acesse:**
- 🌐 **Frontend**: http://localhost:3000
- 🔧 **API/Swagger**: http://localhost:5000/swagger
- 📊 **RabbitMQ**: http://localhost:15672 (guest/guest)

**Parar tudo:**
```powershell
docker-compose down
```

📖 **Guia completo:** [COMO-RODAR.md](COMO-RODAR.md)

---

## 📖 Sobre o Projeto

Sistema **full-stack** que demonstra boas práticas de desenvolvimento:

### **Backend (.NET 10)**
- ✅ **API REST** com ASP.NET Core e Swagger
- ✅ **Mensageria assíncrona** com RabbitMQ
- ✅ **Worker Service** (BackgroundService) processando fila
- ✅ **Persistência** com Entity Framework Core e SQL Server
- ✅ **Clean Architecture** (Domain, Application, Infrastructure, API, Worker)
- ✅ **Logging estruturado** com Serilog
- ✅ **Testes unitários** com xUnit, Moq e FluentAssertions

### **Frontend (React + Vite)**
- ✅ **React 18** com TypeScript
- ✅ **Vite** para build ultrarrápido
- ✅ **Axios** para comunicação com a API
- ✅ **Interface responsiva** e validações (nome sem números, data automática, valor decimal)

### **DevOps**
- ✅ **Docker** e **Docker Compose** para todos os serviços
- ✅ **CI/CD** com GitHub Actions (build, test, push)
- ✅ **Multi-stage builds** otimizados
- ✅ **Automated testing** em Pull Requests
- ✅ **Docker Hub** auto-publish em push para main

---

## 🏗️ Arquitetura

```
┌──────────────────┐
│    Frontend      │
│  (React + Vite)  │ :3000
└────────┬─────────┘
		 │ HTTP
		 ↓
┌────────────────────────────────────────┐
│              API                       │
│         (ASP.NET Core 10)              │ :5000
│  ┌──────────────┐   ┌──────────────┐  │
│  │ Controllers  │───│  Services    │  │
│  └──────────────┘   └──────┬───────┘  │
└─────────────────────────────┼──────────┘
		 │                    │
		 ↓                    ↓
┌─────────────────┐   ┌──────────────┐
│   SQL Server    │   │   RabbitMQ   │
│   (Database)    │   │    (Queue)   │
└─────────────────┘   └──────┬───────┘
		 ↑                    │
		 │                    ↓
		 │            ┌───────────────────┐
		 └────────────│      Worker       │
					  │ (BackgroundService)│
					  └───────────────────┘
```

### **Fluxo de Processamento**

1. **Frontend** envia pedido → **API**
2. **API** salva com `Status = Pending` → **SQL Server**
3. **API** publica mensagem → **RabbitMQ**
4. **Worker** consome da fila
5. **Worker** marca como `Processed` → **SQL Server**
6. **Frontend** atualiza lista (botão refresh)

---

## 🛠️ Tecnologias

### Backend
| Tecnologia | Versão | Uso |
|------------|--------|-----|
| .NET | 10.0 | Framework principal |
| ASP.NET Core | 10.0 | Web API |
| Entity Framework Core | 10.0 | ORM |
| SQL Server | 2022 | Database |
| RabbitMQ | 3-management | Message broker |
| Serilog | Latest | Logging |
| xUnit + Moq | Latest | Testes |

### Frontend
| Tecnologia | Versão | Uso |
|------------|--------|-----|
| React | 18.3.x | UI Library |
| TypeScript | 5.6.x | Type safety |
| Vite | 6.2.x | Build tool |
| Axios | 1.7.x | HTTP client |
| Nginx | Alpine | Web server |

### DevOps
| Tecnologia | Uso |
|------------|-----|
| Docker | Containerização |
| Docker Compose | Orquestração |
| GitHub Actions | CI/CD |

---

## 📂 Estrutura do Projeto

```
OrderTracking/
├── src/
│   ├── OrderTracking.Api/              # API REST
│   ├── OrderTracking.Application/      # Lógica de negócio
│   ├── OrderTracking.Domain/           # Entidades e regras
│   ├── OrderTracking.Infrastructure/   # Implementações técnicas
│   ├── OrderTracking.Worker/           # BackgroundService
│   └── OrderTracking.Web/              # Frontend React + Vite
│
├── tests/
│   └── OrderTracking.Tests/            # Testes unitários
│
├── .github/workflows/                  # CI/CD
│   ├── ci.yml                          # Build & Test
│   └── ci-cd.yml                       # Build & Push Docker
│
├── docker-compose.yml                  # Todos os serviços
├── COMO-RODAR.md                       # Guia rápido
└── README.md                           # Este arquivo
```

**Princípios:**
- ✅ Clean Architecture
- ✅ SOLID
- ✅ Domain-Driven Design
- ✅ Dependency Inversion

---

## 📡 Endpoints da API

### **POST /api/orders**
Cria um novo pedido.

```json
// Request
{
  "customerName": "João Silva",
  "amount": 150.50
}

// Response (200)
{
  "id": "guid",
  "customerName": "João Silva",
  "amount": 150.50,
  "orderDate": "2024-05-27T14:30:00Z",
  "status": 0,  // 0=Pending, 1=Processed
  "createdAt": "2024-05-27T14:30:00Z"
}
```

### **GET /api/orders**
Lista todos os pedidos.

### **GET /api/orders/{id}**
Busca pedido por ID.

📖 **Documentação completa**: http://localhost:5000/swagger

---

## 🧪 Testes

### **Rodar testes unitários:**
```powershell
dotnet test
```

**Cobertura:**
- ✅ Domain: `OrderTests` (criação, processamento)
- ✅ Application: `OrderServiceTests` (CRUD, publicação)
- ✅ Total: 6 testes passando

**Frameworks:**
- xUnit (test runner)
- Moq (mocking)
- FluentAssertions (assertions)

---

## 🐳 Docker

### **Serviços:**
- `sqlserver` - SQL Server 2022
- `rabbitmq` - RabbitMQ 3 + Management UI
- `api` - API .NET 10
- `worker` - BackgroundService consumidor
- `web` - Frontend React

### **Comandos úteis:**
```powershell
# Build e start
docker-compose up -d --build

# Logs
docker-compose logs -f
docker-compose logs -f api
docker-compose logs -f worker

# Parar
docker-compose down

# Limpar tudo
docker-compose down -v
docker system prune -a
```

---

## 🔄 CI/CD

O projeto possui **3 workflows automatizados**:

### **1. CI - Build & Test** (`ci.yml`)
**Trigger:** Pull Requests

**Executa:**
- ✅ Análise de código (.NET format)
- ✅ Build backend + frontend
- ✅ Testes unitários com cobertura
- ✅ Validação de Dockerfiles
- ✅ Comentário automático com cobertura no PR

### **2. CI/CD Pipeline** (`ci-cd.yml`)
**Trigger:** Push para `main`

**Executa:**
- ✅ Build & Test completo
- ✅ Build de imagens Docker multi-arch (amd64 + arm64)
- ✅ Push automático para Docker Hub
- ✅ Tags: `latest`, `main-sha`, versão semver

### **3. Deploy** (`deploy.yml`)
**Trigger:** Manual (workflow_dispatch)

**Permite:**
- 🎯 Deploy controlado para production/staging
- 🏷️ Escolha de versão específica
- 📋 Summary completo do deploy

---

### **⚡ Setup Rápido (5 minutos)**

**1. Criar conta no Docker Hub**
- Acesse: https://hub.docker.com
- Crie conta grátis

**2. Criar Access Token**
- Docker Hub → Settings → Security → New Access Token
- Permissions: Read, Write, Delete

**3. Configurar Secrets no GitHub**
```
Settings → Secrets and variables → Actions → New secret:

DOCKER_USERNAME = seu_username_dockerhub
DOCKER_PASSWORD = dckr_pat_abc123... (o token)
```

**4. Push para main**
```bash
git push origin main
```

**5. Verificar**
- GitHub → Actions (veja o workflow rodando)
- Docker Hub (imagens publicadas)

---

### **📦 Usando as Imagens Publicadas**

```bash
# Pull das imagens
docker pull seu_username/ordertracking-api:latest
docker pull seu_username/ordertracking-worker:latest
docker pull seu_username/ordertracking-web:latest

# Ou usar docker-compose.prod.yml
export DOCKER_USERNAME=seu_username
docker-compose -f docker-compose.prod.yml pull
docker-compose -f docker-compose.prod.yml up -d
```

---

### **📖 Guias Detalhados**

- ⚡ **Quick Start**: [QUICK-START-CI-CD.md](QUICK-START-CI-CD.md) - Setup em 5 min
- 📚 **Guia Completo**: [GITHUB-ACTIONS-GUIDE.md](GITHUB-ACTIONS-GUIDE.md) - Tudo sobre CI/CD
- 🐳 **Docker**: [DOCKER-GUIDE.md](DOCKER-GUIDE.md) - Guia Docker completo

---

## 📚 Documentação Adicional

- ⚡ [QUICK-START-CI-CD.md](QUICK-START-CI-CD.md) - Setup CI/CD em 5 minutos
- 📚 [GITHUB-ACTIONS-GUIDE.md](GITHUB-ACTIONS-GUIDE.md) - Guia completo de CI/CD
- 🚀 [COMO-RODAR.md](COMO-RODAR.md) - Guia rápido de execução
- 🐳 [DOCKER-GUIDE.md](DOCKER-GUIDE.md) - Guia completo Docker
- 🏗️ [REFATORACAO-CONSUMER-WORKER.md](REFATORACAO-CONSUMER-WORKER.md) - Decisões arquiteturais

---

## 🐛 Troubleshooting

### **Docker não inicia:**
```powershell
# Verificar se Docker está rodando
docker info

# Limpar containers antigos
docker-compose down -v
docker system prune -a
```

### **Porta já em uso:**
Edite `docker-compose.yml` e troque as portas:
```yaml
ports:
  - "5001:8080"  # API na porta 5001
  - "3001:80"    # Frontend na porta 3001
```

### **Banco vazio:**
Aguarde 30-60 segundos após `docker-compose up`.
O Entity Framework cria o banco automaticamente.

### **RabbitMQ não conecta:**
```powershell
# Verificar logs
docker-compose logs rabbitmq

# Restart RabbitMQ
docker-compose restart rabbitmq
```

---

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais e de demonstração.

---

## 👤 Autor

**Wendry Andrade**
- GitHub: [@wendryandrade](https://github.com/wendryandrade)
- Repositório: [order-tracking](https://github.com/wendryandrade/order-tracking)

---

## 🎯 Features Implementadas

- [x] API REST com Swagger
- [x] Mensageria RabbitMQ
- [x] Worker Service processador
- [x] Persistência SQL Server + EF Core
- [x] Frontend React + Vite
- [x] Testes unitários
- [x] Docker Compose
- [x] CI/CD GitHub Actions
- [x] Clean Architecture
- [x] Logging com Serilog
- [x] Validações de entrada
- [x] Health checks
- [x] Multi-stage Docker builds

---

🎉 **Projeto pronto para produção!**
