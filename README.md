# 📦 Order Tracking

Sistema completo de rastreamento de pedidos desenvolvido com **.NET 10**, **RabbitMQ**, **SQL Server**, **React + Vite** e **Clean Architecture**, utilizando mensageria para processamento assíncrono.

---

## 🚀 Início Rápido (Como rodar)

**Clone o repositório**

```powershell
git clone https://github.com/wendryandrade/order-tracking.git
```

**Entre na pasta do projeto**
```powershell
cd order-tracking
```

**Rode o comando docker**
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

---

## 📖 Sobre o Projeto

Sistema **full-stack** que demonstra boas práticas de desenvolvimento:

### **Backend (.NET 10)**
- ✅ **API REST** com ASP.NET Core e Swagger
- ✅ **Mensageria assíncrona** com RabbitMQ
- ✅ **Worker Service** (BackgroundService) processando fila
- ✅ **Persistência** com Entity Framework Core e SQL Server
- ✅ **Clean Architecture** (Domain, Application, Infrastructure, API, Worker)
- ✅ **Testes unitários** com xUnit, Moq e FluentAssertions

### **Frontend (React + Vite)**
- ✅ **React 18** com TypeScript
- ✅ **Vite** para build ultrarrápido
- ✅ **Axios** para comunicação com a API
- ✅ **Interface responsiva** e validações (nome sem números, data automática, valor decimal)

### **DevOps**
- ✅ **Docker** e **Docker Compose** para todos os serviços

---

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
├── docker-compose.yml                  # Todos os serviços
└── README.md                           # Este arquivo
```

**Princípios:**
- ✅ Clean Architecture
- ✅ SOLID
- ✅ Domain-Driven Design

---

## 📡 Endpoints da API

### **POST /api/orders**
Cria um novo pedido.

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

---

## 🐳 Docker

### **Serviços:**
- `sqlserver` - SQL Server 2022
- `rabbitmq` - RabbitMQ 3 + Management UI
- `api` - API .NET 10
- `worker` - BackgroundService consumidor
- `web` - Frontend React

---

## 👤 Autor

**Wendrya Andrade**
